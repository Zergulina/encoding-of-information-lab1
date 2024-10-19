using Lab1;
using Microsoft.VisualBasic;

string contents = File.ReadAllText(@"D:\Загрузки\mumu.txt");
System.IO.File.WriteAllText(@"D:\Загрузки\mumu_zip.txt", Haffman.Encode(contents), System.Text.Encoding.Unicode);
string contents_zip = File.ReadAllText(@"D:\Загрузки\mumu_zip.txt", System.Text.Encoding.Unicode);
System.Console.WriteLine(Haffman.Decode( Haffman.Encode(contents)));
