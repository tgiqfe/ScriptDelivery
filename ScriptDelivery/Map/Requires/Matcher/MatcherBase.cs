using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ScriptDelivery.Lib;

namespace ScriptDelivery.Map.Requires.Matcher
{
    internal class MatcherBase
    {

        /// <summary>
        /// 文字列情報から各パラメータをセット
        /// </summary>
        /// <param name="param"></param>
        public void SetParam(Dictionary<string, string> param)
        {
            if (param == null) { return; }

            var props = this.GetType().GetProperties(
                BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var paramAttr = prop.GetCustomAttribute<MatcherParameterAttribute>();
                if (paramAttr == null) { continue; }

                var keys = prop.GetCustomAttribute<KeysAttribute>();
                string matchKey = keys?.GetCandidate().FirstOrDefault(x => param.ContainsKey(x));
                if (matchKey != null)
                {
                    string matchValue = param[matchKey];
                    Type type = prop.PropertyType;

                    if (type == typeof(string))
                    {
                        SetString(prop, matchValue);
                    }
                    else if (type == typeof(bool?))
                    {
                        SetBool(prop, matchValue);
                    }
                    else if (type == typeof(int?))
                    {
                        SetInt(prop, matchValue, paramAttr.Unsigned);
                    }
                    else if (type == typeof(DateTime?))
                    {
                        SetDateTime(prop, matchValue);
                    }
                    else if (type == typeof(string[]))
                    {
                        SetStrings(prop, matchValue, paramAttr.Delimiter);
                    }
                    else if (type == typeof(Dictionary<string, string>))
                    {
                        SetDictionary(prop, matchValue, paramAttr.Delimiter, paramAttr.EqualSign);
                    }
                    else if ((type = Nullable.GetUnderlyingType(type)).IsEnum)
                    {
                        SetEnum(prop, matchValue, type);
                    }
                }
            }
        }

        #region Set paremter pivate methods

        private void SetString(PropertyInfo prop, string val)
        {
            prop.SetValue(this, val);
        }

        public void SetBool(PropertyInfo prop, string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                prop.SetValue(this, !BooleanCandidate.IsFalse(val));
            }
        }

        public void SetInt(PropertyInfo prop, string val, bool unsigned)
        {
            if (int.TryParse(val, out int tempInt))
            {
                if (!unsigned || tempInt >= 0)
                {
                    prop.SetValue(this, tempInt);
                }
            }
        }

        public void SetDateTime(PropertyInfo prop, string val)
        {
            prop.SetValue(this, DateTime.TryParse(val, out DateTime dt) ? dt : null);
        }

        public void SetStrings(PropertyInfo prop, string val, char pDelimiter)
        {
            char delimiter = val.Contains(pDelimiter) ? pDelimiter : '\n';
            string[] array = val.Split(delimiter).
                Select(x => x.Trim()).
                ToArray();
            prop.SetValue(this, array);
        }

        public void SetDictionary(PropertyInfo prop, string val, char pDelimiter, char equalSign)
        {
            var dictionary = new Dictionary<string, string>();

            char delimiter = val.Contains(pDelimiter) ? pDelimiter : '\n';
            val.Split(delimiter).
                Select(x => x.Trim()).
                Where(x => x.Contains(equalSign)).
                ToList().
                ForEach(x =>
                {
                    string itemKey = x.Substring(0, x.IndexOf(equalSign)).Trim();
                    string itemValue = x.Substring(x.IndexOf(equalSign) + 1).Trim();
                    dictionary[itemKey] = itemValue;
                });

            prop.SetValue(this, dictionary);
        }

        public void SetEnum(PropertyInfo prop, string val, Type type)
        {
            var values = prop.GetCustomAttribute<ValuesAttribute>();
            if (values != null)
            {
                foreach (SubCandidate candidate in values.GetCandidate())
                {
                    string enumText = candidate.Check(val);
                    if (enumText != null)
                    {
                        prop.SetValue(this, Enum.Parse(type, enumText, true));
                    }
                }
            }
        }

        #endregion
    }
}
