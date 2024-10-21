using Lab1;

string contents = File.ReadAllText(@"C:\Users\zergu\Downloads\Telegram Desktop\mumu.txt", System.Text.Encoding.Unicode);
File.WriteAllBytes(@"C:\Users\zergu\Downloads\Telegram Desktop\mumu_zip.txt", Haffman.Encode(Lzw.Encode(contents, 4)));
var zip_contents = File.ReadAllBytes(@"C:\Users\zergu\Downloads\Telegram Desktop\mumu_zip.txt");
Console.WriteLine(Lzw.Decode(Haffman.Decode(zip_contents)).Equals(contents));