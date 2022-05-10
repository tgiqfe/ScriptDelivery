using System;
using System.Text;
using ScriptDelivery;
using System.Runtime.InteropServices;
using System.Diagnostics;

/// <summary>
/// 静的パラメータを格納
/// </summary>
namespace ScriptDelivery.Server.ServerLib
{
    internal class Item
    {
        /// <summary>
        /// 現在実行中のOSの種類
        /// Windows/Linux対応。
        /// </summary>
        private static OSPlatform _platform = OSPlatform.FreeBSD;

        /// <summary>
        /// 初期値FreeBSDであれば、初回アクセス時にOS判定をする
        /// </summary>
        public static OSPlatform Platform
        {
            get
            {
                if (_platform == OSPlatform.FreeBSD)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        _platform = OSPlatform.Windows;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        _platform = OSPlatform.Linux;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        _platform = OSPlatform.OSX;
                    }
                }
                return _platform;
            }
        }

        private static Setting _setting = null;

        public static Setting Setting
        {
            get
            {
                _setting ??= Setting.Deserialize("setting.json");
                return _setting;
            }
        }

        private static MappingDB _mappingDB = null;

        public static MappingDB MappingDB
        {
            get
            {
                _mappingDB ??= new MappingDB(_setting.MapsPath);
                return _mappingDB;
            }
        }
    }
}
