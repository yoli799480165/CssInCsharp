using System;
using System.Collections.Generic;

namespace CssInCSharp.Extensions
{
    public static class TypeExtensions
    {
        private const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static int CharCodeAt(this string str, int index)
        {
            return str[index];
        }

        public static string Hash(this string str)
        {
            long h = 0;
            int k = 0;
            var i = 0;
            var len = str.Length;
            for (; len >= 4; ++i, len -= 4)
            {
                k = (str.CharCodeAt(i) & 0xff) |
                    ((str.CharCodeAt(++i) & 0xff) << 8) |
                    ((str.CharCodeAt(++i) & 0xff) << 16) |
                    ((str.CharCodeAt(++i) & 0xff) << 24);

                k = (k & 0xffff) * 0x5bd1e995 + (((k >>> 16) * 0xe995) << 16);
                k ^= k >>> 24;
                h = ((k & 0xffff) * 0x5bd1e995 + (((k >>> 16) * 0xe995) << 16)) ^
                    ((h & 0xffff) * 0x5bd1e995 + (((h >>> 16) * 0xe995) << 16));
            }
            switch (len)
            {
                case 3:
                    h ^= (str.CharCodeAt(i + 2) & 0xff) << 16;
                    goto case 2;
                case 2:
                    h ^= (str.CharCodeAt(i + 1) & 0xff) << 8;
                    goto case 1;
                case 1:
                    h ^= str.CharCodeAt(i) & 0xff;
                    h = ((h & 0xffff) * 0x5bd1e995) + (((int)(h >>> 16) * 0xe995) << 16);
                    break;
            }
            h ^= (int)h >>> 13;
            h = ((h & 0xffff) * 0x5bd1e995) + (((int)(h >>> 16) * 0xe995) << 16);
            var val = ((int)(h ^ ((int)h >>> 15)));
            return Convert.ToInt64(Convert.ToString((val >>> 0), toBase: 2), 2).ToString(36);
        }

        public static string ToString(this long value, int radix = 36)
        {
            var clistarr = CharList.ToCharArray();
            var result = new Stack<char>();
            while (value != 0)
            {
                result.Push(clistarr[value % radix]);
                value /= radix;
            }
            return new string(result.ToArray());
        }
    }
}
