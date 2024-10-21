using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    internal class Lzw
    {
        public static string Encode(string str, ulong dictionaryCapacity)
        {
            string encode = dictionaryCapacity.ToString();
            var (startDictionary, dictionaryOffset) = GetSymbolDictionary(str);

            foreach (var codePair in startDictionary)
            {
                encode += "@" + codePair.Key + codePair.Value;
            }
            encode += "|";

            string buffer = "";

            var bufferDictionary = new Dictionary<string, ulong>();

            ulong counter = 0;
    
            for (var i = 0; i < str.Length - 1; i++)
            {
                buffer += str[i];
                if (bufferDictionary.ContainsKey(buffer + str[i + 1])) continue;
                if (buffer.Length == 1) encode += startDictionary[buffer[0]];
                else encode += bufferDictionary[buffer] + dictionaryOffset;
                var index = counter % dictionaryCapacity;
                if (bufferDictionary.ContainsValue(index))
                {
                    bufferDictionary.Remove(bufferDictionary.First(x => x.Value == index).Key);
                }
                bufferDictionary.Add(buffer + str[i+1], index);
                encode += "|";
                buffer = "";
                counter++;
            }

            if (buffer.Length == 1) encode += startDictionary[buffer[0]] + "|";
            else if (buffer.Length > 1) encode += bufferDictionary[buffer + str[str.Length - 1]] + dictionaryOffset + "|";

            return encode;
        }

        public static string Decode(string str)
        {
            string decode = "";
            var (startDictionary, dictionaryOffset, offset, dictionaryCapacity) = GetSymbolDictionaryFromCode(str);

            var bufferDictionary = new Dictionary<ulong, string>();

            string strBuffer = "";

            string codeBuffer = "";

            ulong counter = 0;

            ulong currentCode = 0;

            for (ulong i = offset; i < (ulong)str.Length; i++)
            {
                if (str[(int)i] == '|')
                {
                    currentCode = ulong.Parse(codeBuffer);
                    codeBuffer = "";
                    if (bufferDictionary.ContainsKey(counter % dictionaryCapacity)) bufferDictionary.Remove(counter % dictionaryCapacity);
                    if (currentCode < dictionaryOffset)
                    {
                        decode += startDictionary[currentCode];
                        if (strBuffer.Length > 0)
                        {
                            bufferDictionary[counter++ % dictionaryCapacity] = strBuffer + startDictionary[currentCode];
                        }
                        strBuffer = startDictionary[currentCode].ToString();
                    }
                    else
                    {
                        if (bufferDictionary.ContainsKey(currentCode - dictionaryOffset))
                        {
                            decode += bufferDictionary[currentCode - dictionaryOffset];
                            bufferDictionary.Add(counter++ % dictionaryCapacity, strBuffer + bufferDictionary[currentCode - dictionaryOffset][0]);
                            strBuffer = bufferDictionary[currentCode - dictionaryOffset].ToString();
                        }
                        else
                        {
                            strBuffer += strBuffer[strBuffer.Length - 1];
                            decode += strBuffer;
                            bufferDictionary.Add(counter++ % dictionaryCapacity, strBuffer);
                        }
                    }
                }
                else codeBuffer += str[(int)i];
            }

            return decode;
        }

        public static (Dictionary<char, ulong>, ulong) GetSymbolDictionary(string str)
        {
            var dict = new Dictionary<char, ulong>();
            ulong counter = 0;
            foreach (char c in str) if (!dict.ContainsKey(c)) dict.Add(c, counter++);
            return (dict, counter);
        }

        public static (Dictionary<ulong, char> , ulong, ulong, ulong) GetSymbolDictionaryFromCode(string str)
        {
            var buffer = "";
            ulong dictionaryCapacity = 0;
            int strOffset = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == '@') 
                {
                    dictionaryCapacity = ulong.Parse(buffer);
                    strOffset = i + 1;
                    break;
                }
                buffer += str[i];
            }

            var symbolDictionary = new Dictionary<ulong, char>();

            char currentChar = '\0';

            ulong dictionaryOffset = 0;
            ulong offset = 0;

            buffer = "";

            for (var i = strOffset; i < str.Length; i++)
            {
                if (str[i-1] == '@' && str[i] == '@') currentChar = '@';
                else if (str[i] == '@' && str[i-1] != '@')
                {
                    symbolDictionary.Add(ulong.Parse(buffer), currentChar);
                    currentChar = ' ';
                    buffer = "";
                    dictionaryOffset++;
                }
                else if (str[i-1] == '@' && currentChar != '@') currentChar = str[i];
                else if (str[i] == '|')
                {
                    dictionaryOffset++;
                    symbolDictionary.Add(ulong.Parse(buffer), currentChar);
                    offset = (ulong)i + 1;
                    break;
                }
                else
                {
                    buffer += str[i];
                }
            }

            return (symbolDictionary, dictionaryOffset, offset, dictionaryCapacity);
        }
    }
}
