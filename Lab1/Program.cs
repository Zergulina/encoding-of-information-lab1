using Lab1;
using System.Text;


string contents = File.ReadAllText(@"D:\Загрузки\mumu.txt");
File.WriteAllText(@"D:\Загрузки\mumu_zip.txt", Haffman.Encode(contents), Encoding.Unicode);
string contents_zip = File.ReadAllText(@"D:\Загрузки\mumu_zip.txt", Encoding.Unicode);
Console.WriteLine(Haffman.Decode( Haffman.Encode(contents)));


