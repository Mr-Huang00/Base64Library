using System;
using System.Text;
using System.Collections;

namespace Base64
{
    static public class Base64Converter
    {

        static public readonly char[] Base64IndexTable = {
        'A' , //0
        'B' , //1
        'C' , //2
        'D' , //3
        'E' , //4
        'F' , //5
        'G' , //6
        'H' , //7
        'I' , //8
        'J' , //9
        'K' , //10
        'L' , //11
        'M' , //12
        'N' , //13
        'O' , //14
        'P' , //15
        'Q' , //16
        'R' , //17
        'S' , //18
        'T' , //19
        'U' , //20
        'V' , //21
        'W' , //22
        'X' , //23
        'Y' , //24
        'Z' , //25
        'a' , //26
        'b' , //27
        'c' , //28
        'd' , //29
        'e' , //30
        'f' , //31
        'g' , //32
        'h' , //33
        'i' , //34
        'j' , //35
        'k' , //36
        'l' , //37
        'm' , //38
        'n' , //39
        'o' , //40
        'p' , //41
        'q' , //42
        'r' , //43
        's' , //44
        't' , //45
        'u' , //46
        'v' , //47
        'w' , //48
        'x' , //49
        'y' , //50
        'z' , //51
        '0' , //52
        '1' , //53
        '2' , //54
        '3' , //55
        '4' , //56
        '5' , //57
        '6' , //58
        '7' , //59
        '8' , //60
        '9' , //61
        '+' , //62
        '/' , //63
        };

        static public readonly string IndexTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        /// <summary>
        /// turn a base64 string into a unicode string.
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        static public string ConvertFromBase64StringToString(string base64Str)
        {
            // Base64字符串解码过程（大致）：
            //      Base64字节数组 -> 将每字节字符通过索引表查询原索引，得到索引数组 -> 将索引数组中多余的部分（一般是额外添加的0）全部判别并去掉，然后合并即可得到源字节数组 -> 源字节数组
            // 实际，逆过程的具体实现比较多，下面我实现所使用的方法就不是我刚才写的那种 :P。
            //
            // 假设Base64字符串长度为L，并且有等号n个，则源字节数组长度为：
            //      n = 0 时：
            //          (L*6)/8
            //      n ≠ 0 时：
            //          (L - n -(4 - n))*6/8 + (4-n) 

            int countOfEqual = 0;
            byte[] srcBA;
            byte[] dstBA;
            int index = 0;
            uint container;

            // count the '='
            if (base64Str[base64Str.Length - 1] == '=')
            {
                countOfEqual++;
                if (base64Str[base64Str.Length - 2] == '=')
                    countOfEqual++;
            }

            // copy the byte array without '=' to the base64 byte array.
            var temp = Encoding.ASCII.GetBytes(base64Str);
            dstBA = new byte[temp.Length - countOfEqual];
            for (int i = 0; i < temp.Length - countOfEqual; i++)
            {
                dstBA[i] = temp[i];
            }

            // init source byte array
            srcBA = new byte[dstBA.Length * 6 / 8];

            // get source byte array from base64 byte array.
            // 4 bytes as 1 group,make the group into 3 bytes group of source,then put them into srcBA;
            for (int i = 0; i < dstBA.Length / 4; i++)
            {
                // init container
                container = 0;
                // push the '6 bit' into container.
                for (int j = 0; j < 4; j++)
                {
                    // (char)dstBA[(i * 4) + j] 用于获取合适位置的字节字符。
                    // IndexTable.IndexOf()用于查询索引表进行逆映射获取索引。
                    container = (uint)(container | ((uint)(IndexTable.IndexOf((char)dstBA[(i * 4) + j]) << 6 * (3 - j))));
                }
                // push the pre 3 byte of container into source byte array.
                srcBA[index++] = (byte)(container >> 16);
                srcBA[index++] = (byte)(container >> 8);
                srcBA[index++] = (byte)container;
            }
            if (countOfEqual != 0)
            {
                if (countOfEqual == 2)
                {
                    srcBA[index] = (byte)((byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 2])) << 2 | (byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 1])) >> 4);
                }
                else if (countOfEqual == 1)
                {
                    srcBA[index++] = (byte)((byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 3])) << 2 | (byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 2])) >> 4);
                    srcBA[index] = (byte)((byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 2])) << 4 | (byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 1])) >> 2);
                }
            }

            return Encoding.Unicode.GetString(srcBA);
        }

        /// <summary>
        /// turn a base64 string into a byte array.
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        static public byte[] ConvertFromBase64String(string base64Str)
        {
            // Base64字符串解码过程（大致）：
            //      Base64字节数组 -> 将每字节字符通过索引表查询原索引，得到索引数组 -> 将索引数组中多余的部分（一般是额外添加的0）全部判别并去掉，然后合并即可得到源字节数组 -> 源字节数组
            // 实际，逆过程的具体实现比较多，下面我实现所使用的方法就不是我刚才写的那种 :P。
            //
            // 假设Base64字符串长度为L，并且有等号n个，则源字节数组长度为：
            //      n = 0 时：
            //          (L*6)/8
            //      n ≠ 0 时：
            //          (L - n - (4 - n))*6/8 + (4-n) 

            int countOfEqual = 0;
            byte[] srcBA;
            byte[] dstBA;
            int index = 0;
            uint container;

            // count the '='
            if (base64Str[base64Str.Length - 1] == '=')
            {
                countOfEqual++;
                if (base64Str[base64Str.Length - 2] == '=')
                    countOfEqual++;
            }

            // copy the byte array without '=' to the base64 byte array.
            var temp = Encoding.ASCII.GetBytes(base64Str);
            dstBA = new byte[temp.Length - countOfEqual];
            for (int i = 0; i < temp.Length - countOfEqual; i++)
            {
                dstBA[i] = temp[i];
            }

            // init source byte array
            srcBA = new byte[dstBA.Length * 6 / 8];

            // get source byte array from base64 byte array.
            // 4 bytes as 1 group,make the group into 3 bytes group of source,then put them into srcBA;
            for (int i = 0; i < dstBA.Length / 4; i++)
            {
                // init container
                container = 0;
                // push the '6 bit' into container.
                for (int j = 0; j < 4; j++)
                {
                    // (char)dstBA[(i * 4) + j] 用于获取合适位置的字节字符。
                    // IndexTable.IndexOf()用于查询索引表进行逆映射获取索引。
                    container = (uint)(container | ((uint)(IndexTable.IndexOf((char)dstBA[(i * 4) + j]) << 6 * (3 - j))));
                }
                // push the pre 3 byte of container into source byte array.
                srcBA[index++] = (byte)(container >> 16);
                srcBA[index++] = (byte)(container >> 8);
                srcBA[index++] = (byte)container;
            }
            if (countOfEqual != 0)
            {
                if (countOfEqual == 2)
                {
                    srcBA[index] = (byte)((byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 2])) << 2 | (byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 1])) >> 4);
                }
                else if (countOfEqual == 1)
                {
                    srcBA[index++] = (byte)((byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 3])) << 2 | (byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 2])) >> 4);
                    srcBA[index] = (byte)((byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 2])) << 4 | (byte)(IndexTable.IndexOf((char)dstBA[dstBA.Length - 1])) >> 2);
                }
            }

            return srcBA;
        }

        /// <summary>
        /// turn a unicode string into a base64 string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static public string ConvertToBase64String(string source)
        {
            //Base64编码过程 ：
            //      源字节数组 -> 从低字节开始，每6位二进制数作为索引，通过查询索引表映射为特定字符 -> Base64编码数组。
            // 其余规则：
            // 1. 剩余长度不足6位时，后补零，即左移。
            // 2. 一般，每3字节为一组。最后一组（余组）不足3字节的话，则在初步得到的base64字符串后面添加等于却是字节数的'='号。即：
            //      * 余组长1字节时，初步得到的base64字符串的末位添加2个等号；
            //      * 余组长2字节时，初步得到的base64字符串的末尾添加1个等号
            //    该规则的目的在于保留源字符串的字节数信息，使得解码时可以无损还原。
            //    (谨记：Base64编码仅仅针对 "以字节为单位"的数组。)
            //
            //设源字节数组长度为L，则
            //Base64加密后的字节数组长度为 ：
            //      (int)(L/3)*4 + (L%3 * 2 + 1)
            //
            byte[] srcBA = Encoding.Unicode.GetBytes(source);
            byte[] dstBA;
            int index = 0;
            byte bitsMask = 0x3f; // get the left 6 bit of a byte or an int type variable.
            int container;
            int rest = srcBA.Length % 3;

            if (rest == 0)
            {
                dstBA = new byte[srcBA.Length * 8 / 6];
            }
            else
            {
                dstBA = new byte[srcBA.Length * 8 / 6 + 3];
            }

            for (int i = 0; i < srcBA.Length / 3; i++)
            {
                container = srcBA[i * 3 + 0] << 16 | srcBA[i * 3 + 1] << 8 | srcBA[i * 3 + 2];
                dstBA[index++] = (byte)IndexTable[container >> 18 & bitsMask];
                dstBA[index++] = (byte)IndexTable[container >> 12 & bitsMask];
                dstBA[index++] = (byte)IndexTable[container >> 6 & bitsMask];
                dstBA[index++] = (byte)IndexTable[container & bitsMask];
            }
            if (rest != 0)
            {
                if (rest == 1)
                {
                    dstBA[index++] = (byte)IndexTable[srcBA[srcBA.Length - 1] >> 2 & bitsMask];
                    dstBA[index++] = (byte)IndexTable[srcBA[srcBA.Length - 1] << 4 & bitsMask];
                    dstBA[index++] = (byte)'=';
                    dstBA[index++] = (byte)'=';
                }
                else if (rest == 2)
                {
                    container = srcBA[srcBA.Length - 2] << 8 | srcBA[srcBA.Length - 1];
                    dstBA[index++] = (byte)IndexTable[container >> 10 & bitsMask];
                    dstBA[index++] = (byte)IndexTable[container >> 4 & bitsMask];
                    dstBA[index++] = (byte)IndexTable[container << 2 & bitsMask];
                    dstBA[index++] = (byte)'=';
                }
            }

            //Log(Encoding.ASCII.GetString(dstBA).GetHashCode() + "");
            //Log(Convert.ToBase64String(srcBA).GetHashCode() + "");

            return Encoding.ASCII.GetString(dstBA);
        }

        /// <summary>
        /// turn byte array into a base64 string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static public string ConvertToBase64String(byte[] source)
        {
            //Base64编码过程 ：
            //      源字节数组 -> 从低字节开始，每6位二进制数作为索引，通过查询索引表映射为特定字符 -> Base64编码数组。
            // 其余规则：
            // 1. 剩余长度不足6位时，后补零，即左移。
            // 2. 一般，每3字节为一组。最后一组（余组）不足3字节的话，则在初步得到的base64字符串后面添加等于却是字节数的'='号。即：
            //      * 余组长1字节时，初步得到的base64字符串的末位添加2个等号；
            //      * 余组长2字节时，初步得到的base64字符串的末尾添加1个等号
            //    该规则的目的在于保留源字符串的字节数信息，使得解码时可以无损还原。
            //    (谨记：Base64编码仅仅针对 "以字节为单位"的数组。)
            //
            //设源字节数组长度为L，则
            //Base64加密后的字节数组长度为 ：
            //      (int)(L/3)*4 + (L%3 * 2 + 1)
            //
            byte[] dstBA;
            int index = 0;
            byte bitsMask = 0x3f; // get the left 6 bit of a byte or an int type variable.
            int container;
            int rest = source.Length % 3;

            if (rest == 0)
            {
                dstBA = new byte[source.Length * 8 / 6];
            }
            else
            {
                dstBA = new byte[source.Length * 8 / 6 + 3];
            }

            for (int i = 0; i < source.Length / 3; i++)
            {
                container = source[i * 3 + 0] << 16 | source[i * 3 + 1] << 8 | source[i * 3 + 2];
                dstBA[index++] = (byte)IndexTable[container >> 18 & bitsMask];
                dstBA[index++] = (byte)IndexTable[container >> 12 & bitsMask];
                dstBA[index++] = (byte)IndexTable[container >> 6 & bitsMask];
                dstBA[index++] = (byte)IndexTable[container & bitsMask];
            }
            if (rest != 0)
            {
                if (rest == 1)
                {
                    dstBA[index++] = (byte)IndexTable[source[source.Length - 1] >> 2 & bitsMask];
                    dstBA[index++] = (byte)IndexTable[source[source.Length - 1] << 4 & bitsMask];
                    dstBA[index++] = (byte)'=';
                    dstBA[index++] = (byte)'=';
                }
                else if (rest == 2)
                {
                    container = source[source.Length - 2] << 8 | source[source.Length - 1];
                    dstBA[index++] = (byte)IndexTable[container >> 10 & bitsMask];
                    dstBA[index++] = (byte)IndexTable[container >> 4 & bitsMask];
                    dstBA[index++] = (byte)IndexTable[container << 2 & bitsMask];
                    dstBA[index++] = (byte)'=';
                }
            }

            //Log(Encoding.ASCII.GetString(dstBA).GetHashCode() + "");
            //Log(Convert.ToBase64String(source).GetHashCode() + "");

            return Encoding.ASCII.GetString(dstBA);
        }


        /// <summary>
        /// Generate base64 index table for using index to get character.
        /// </summary>
        /// <returns></returns>
        static public char[] GenerateIndexTable_CharArray()
        {
            long n = 0;
            char[] table = new char[64];
            //'A' ~ 'Z'
            char starChar = 'A';
            for (int i = 0; i < 'Z' - 'A' + 1; i++)
            {
                table[n++] = starChar++;
            }

            //'a' ~ 'z'
            starChar = 'a';
            for (int i = 0; i < 'z' - 'a' + 1; i++)
            {
                table[n++] = starChar++;
            }

            //'0' ~ '9'
            starChar = '0';
            for (int i = 0; i < '9' - '0' + 1; i++)
            {
                table[n++] = starChar++;
            }

            table[n++] = '+';
            table[n] = '/';

            return table;
        }

        /// <summary>
        /// Generate base64 hash table for using character to get index.
        /// </summary>
        /// <returns></returns>
        static public Hashtable GenerateIndexTableD_HashTable()
        {
            long n = 0;
            Hashtable table = new Hashtable(64);
            //'A' ~ 'Z'
            char starChar = 'A';
            for (int i = 0; i < 'Z' - 'A' + 1; i++)
            {
                table[starChar++] = n++;
            }

            //'a' ~ 'z'
            starChar = 'a';
            for (int i = 0; i < 'z' - 'a' + 1; i++)
            {
                table[starChar++] = n++;
            }

            //'0' ~ '9'
            starChar = '0';
            for (int i = 0; i < '9' - '0' + 1; i++)
            {
                table[starChar++] = n++;
            }

            table['+'] = n++;
            table['/'] = n;

            return table;
        }

        /// <summary>
        /// Generate code for defining index table.(char[] array and string)
        /// </summary>
        static public void GenerateCode()
        {
            // 一般方法，生成字符数组表
            var table = GenerateIndexTable_CharArray();
            Console.Write("char[] table = {");
            for (int i = 0; i < table.Length; i++)
            {
                Console.WriteLine($"\'{table[i]}\' , //{i}");
            }
            Console.Write("};");

            Console.WriteLine("\r\n  ----------------- \r\n");
            // 邪门歪道
            //考虑到char[]用来根据字符查序列时要自己编写方法，此时希望另寻门道。
            //考虑到字符串"abcdef"可以像char[]一样进行索引也可以调用方法.indexof来进行查询，所以可以使用字符串类来作为查询表。
            Console.Write("string table = \"");
            for (int i = 0; i < table.Length; i++)
            {
                Console.Write($"{table[i]}");
            }
            Console.Write("\";");
        }

        /// <summary>
        /// Check the validation of base64.
        /// </summary>
        /// <param name="base64str"></param>
        /// <returns></returns>
        static public bool CheckBase64(string base64str)
        {
            int countOfEqual = 0;
            if (base64str[base64str.Length - 1] == '=')
            {
                countOfEqual++;
                if (base64str[base64str.Length - 2] == '=')
                    countOfEqual++;
            }

            //验证长度
            if (countOfEqual == 0)
            {
                if ((base64str.Length * 3) % 4 != 0)
                {
                    ErrorCode = EC_InvalidLength;
                    return false;
                }
            }
            else
            {
                if ((((base64str.Length - countOfEqual - (4 - countOfEqual)) * 3) % 4) != 0)
                {
                    ErrorCode = EC_InvalidLength;
                    return false;
                }
            }

            // 验证字符
            //var c = base64str.Substring(0, base64str.Length - countOfEqual).Where(
            //    (e) => {
            //        if (!IndexTable.Contains(e)) 
            //            return false;
            //        return true; }
            //    );
            //if (c.Count() != 0)
            //    return false;
            //else
            //    return true;

            for (int i = 0; i < base64str.Length - countOfEqual; i++)
            {
                if (!IndexTable.Contains(base64str[i]))
                {
                    ErrorCode = EC_InvalidCharacter;
                    return false;
                }
            }
            return true;
        }

        static public int ErrorCode { get; private set; }

        // Error Code
        static public readonly int EC_OK = 0;

        static public readonly int EC_InvalidLength = 1;

        static public readonly int EC_InvalidCharacter = 2;
    }
}
