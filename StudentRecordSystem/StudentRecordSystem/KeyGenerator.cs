using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace StudentRecordSystem
{
    class KeyGenerator
    {
        static char[] chars = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();

        public static string GetUniqueKey()
        {
            /** The Of Generated Password String **/
            const int length = 12;
            byte[] data = new byte[4 * length];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(data);

            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }
    }
}
