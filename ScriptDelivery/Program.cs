


string[] array1 = new string[] { "aaaa", "bbbb", "cccc" };
string[] array2 = new string[] { "dddd", "eeee", "ffff" };
string[] array3 = new string[] { "AAAA", "BBBB", "CCCC" };
string[] array4 = new string[] { "DDDD", "EEEE", "FFFF" };

string[][] total = new string[][] {
    array1,
    array2,
    array3,
    array4,
};

var ret = total.Aggregate((x, y) => x.Concat(y).ToArray());

Console.WriteLine(string.Join("-", ret));





Console.ReadLine();

