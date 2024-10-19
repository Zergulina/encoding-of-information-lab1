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
        public static string Encode(String str)
        {
            var symbolDictionary = CountSymbolsInText(str);
            var tree = new HaffmanTree(symbolDictionary);
            string encode = "";
            foreach (var codePair in symbolDictionary) 
            {
                encode += "@" + codePair.Key.ToString() + codePair.Value.ToString();
            }
            encode += "|";
            UInt16 symbol = 0;
            int digitCounter = 0;
            uint bitmask = 0b1000_0000_0000_0000;
            foreach (char c in str)
            {
                foreach (var bit in tree.Codes[c])
                {
                    if (bit) symbol |= (ushort)bitmask;
                    bitmask = bitmask >> 1;

                    digitCounter++;
                    if (digitCounter == 16) 
                    {
                        bitmask = 0b1000_0000_0000_0000;
                        encode += Convert.ToChar(symbol);
                        digitCounter = 0;
                        symbol = 0;
                    }
                }
            }
            Console.WriteLine();
            encode += Convert.ToChar(symbol);
            encode += Convert.ToString(digitCounter, 16);
            return encode;
        }

        public static string Decode(String str)
        {
            var (symbolDictionary, offset) = CountSymbolsInCode(str);
            var tree = new HaffmanTree(symbolDictionary);
            string decode = ""; 
            uint bitmask = 0b1000_0000_0000_0000;
            var currentCode = new List<bool>();
            var backBias = int.Parse(str[str.Length - 1].ToString(), System.Globalization.NumberStyles.HexNumber);
            for (var i = offset; i < str.Length - 2; i++) 
            {
                for (short digitCounter = 0; digitCounter < 16; digitCounter++)
                {
                    currentCode.Add(((UInt16)str[i] & bitmask) == bitmask);
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

                bitmask = 0b1000_0000_0000_0000;
            }
            for (short digitCounter = 0; digitCounter < backBias; digitCounter++)
            {
                currentCode.Add(((UInt16)str[str.Length-2] & bitmask) == bitmask);
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

        public static (Dictionary<char, ulong>, int) CountSymbolsInCode(String str)
        {
            var symbolDictionary = new Dictionary<char, ulong>();
            char currentChar = ' ';
            string number = "";
            int offset;
            for (int i = 1; true; i++)
            {
                if (str[i] == '@')
                {
                    symbolDictionary.Add(currentChar, ulong.Parse(number));
                    number = "";
                }
                else if (str[i - 1] == '@') currentChar = str[i];
                else if (str[i] == '|')
                {
                    symbolDictionary.Add(currentChar, ulong.Parse(number));
                    offset = i + 1;
                    break;
                }
                else
                {
                    number += str[i];
                }
            }

            return (symbolDictionary, offset); 
        }
    }
}
