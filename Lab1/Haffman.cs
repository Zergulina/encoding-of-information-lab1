using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    internal class Haffman
    {

        public static byte[] Encode(string str)
        {
            var symbolDictionary = CountSymbolsInText(str);
            var tree = new HaffmanTree(symbolDictionary);

            LinkedList<byte> encode = new LinkedList<byte>();
            foreach (var codePair in symbolDictionary)
            {
                encode.AddLast((byte)('@' >> 8));
                encode.AddLast((byte)'@');

                encode.AddLast((byte)(codePair.Key >> 8));
                encode.AddLast((byte)codePair.Key);

                foreach (var c in codePair.Value.ToString())
                {
                    encode.AddLast((byte)(c >> 8));
                    encode.AddLast((byte)c);
                }
            }
            encode.AddLast((byte)('|' >> 8));
            encode.AddLast((byte)'|');
            byte symbol = 0;
            int digitCounter = 0;
            int bitmask = 0b1000_0000;
            foreach (char c in str)
            {
                foreach (var bit in tree.Codes[c])
                {
                    if (bit) symbol |= (byte)bitmask;
                    bitmask = bitmask >> 1;

                    digitCounter++;
                    if (digitCounter == 8)
                    {
                        bitmask = 0b1000_0000;
                        encode.AddLast(symbol);
                        digitCounter = 0;
                        symbol = 0;
                    }
                }
            }
            if (digitCounter > 0) encode.AddLast(symbol);
            char leftDigit = Convert.ToString(digitCounter, 8).Last();
            encode.AddLast((byte)(leftDigit >> 8));
            encode.AddLast((byte)leftDigit);
            return encode.ToArray();
        }


        public static string Decode(byte[] str)
        {
            var (symbolDictionary, offset) = CountSymbolsInCode(str);
            var tree = new HaffmanTree(symbolDictionary);
            string decode = ""; 
            uint bitmask = 0b1000_0000;
            var currentCode = new List<bool>();
            var backBias = ushort.Parse(((char)int.Parse(Convert.ToString(str[str.Length - 2], 2).PadLeft(8, '0') + Convert.ToString(str[str.Length - 1], 2).PadLeft(8, '0'), System.Globalization.NumberStyles.BinaryNumber)).ToString());
            for (var i = offset; i < str.Length - 3; i++) 
            {
                for (short digitCounter = 0; digitCounter < 8; digitCounter++)
                {
                    currentCode.Add((str[i] & bitmask) == bitmask);
                    foreach (var code in tree.Codes)
                    {
                        if (currentCode.SequenceEqual(code.Value))
                        {
                            decode += code.Key;
                            currentCode.Clear();
                        }
                    }

                    bitmask = bitmask >> 1;
                }

                bitmask = 0b1000_0000;
            }
            for (short digitCounter = 0; digitCounter < backBias; digitCounter++)
            {
                currentCode.Add((str[str.Length-3] & bitmask) == bitmask);
                foreach (var code in tree.Codes)
                {
                    if (currentCode.SequenceEqual(code.Value))
                    {
                        decode += code.Key;
                        currentCode.Clear();
                    }
                }

                bitmask = bitmask >> 1;
            }
            return decode;
        }

        private static Dictionary<char, ulong> CountSymbolsInText(String str)
        {
            Dictionary<char, ulong> symbolDictionary = new Dictionary<char, ulong>();
            foreach (char symbol in str)
            {
                if (symbolDictionary.ContainsKey(symbol))
                    symbolDictionary[symbol]++;
                else
                    symbolDictionary[symbol] = 1;
            }

            return symbolDictionary;
        }

        public static (Dictionary<char, ulong>, int) CountSymbolsInCode(byte[] str)
        {
            var symbolDictionary = new Dictionary<char, ulong>();
            char currentChar = ' ';
            string number = "";
            int offset;
            byte[] prevCharStrBytes = new byte[2];
            byte[] currentCharStrBytes = new byte[2];
            char prevCharStr = ' ';
            char currentCharStr = ' ';
            for (int i = 3; true; i+=2)
            {
                prevCharStrBytes[0] = str[i - 3];
                prevCharStrBytes[1] = str[i - 2];
                prevCharStr = (char)ushort.Parse(Convert.ToString(prevCharStrBytes[0], 2).PadLeft(8, '0') + Convert.ToString(prevCharStrBytes[1], 2).PadLeft(8, '0'), System.Globalization.NumberStyles.BinaryNumber);

                currentCharStrBytes[0] = str[i - 1];
                currentCharStrBytes[1] = str[i];
                currentCharStr = (char)ushort.Parse(Convert.ToString(currentCharStrBytes[0], 2).PadLeft(8, '0') + Convert.ToString(currentCharStrBytes[1], 2).PadLeft(8, '0'), System.Globalization.NumberStyles.BinaryNumber);

                if (prevCharStr == '@' && currentCharStr == '@') currentChar = '@';
                else if (currentCharStr == '@' && prevCharStr != '@')
                {
                    symbolDictionary.Add(currentChar, ulong.Parse(number));
                    currentChar = ' ';
                    number = "";
                }
                else if (prevCharStr == '@' && currentChar!='@') currentChar = currentCharStr;
                else if (currentCharStr == '|')
                {
                    symbolDictionary.Add(currentChar, ulong.Parse(number));
                    offset = i + 1;
                    break;
                }
                else
                {
                    number += currentCharStr;
                }
            }

            return (symbolDictionary, offset); 
        }
    }
}
