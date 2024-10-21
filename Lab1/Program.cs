using Lab1;

string contents = File.ReadAllText(@"C:\Users\zergu\Downloads\Telegram Desktop\mumu.txt", System.Text.Encoding.Unicode);
File.WriteAllText(@"C:\Users\zergu\Downloads\Telegram Desktop\mumu2.txt", contents, System.Text.Encoding.Unicode);
File.WriteAllText(@"C:\Users\zergu\Downloads\Telegram Desktop\mumu_zip.txt", Haffman.Encode(contents), System.Text.Encoding.Unicode);
string contents_zip = File.ReadAllText(@"C:\Users\zergu\Downloads\Telegram Desktop\mumu_zip.txt", System.Text.Encoding.Unicode);
Console.WriteLine(contents_zip.Equals(Haffman.Encode(contents)));
//Console.WriteLine(Haffman.Decode(Haffman.Encode(contents)));
