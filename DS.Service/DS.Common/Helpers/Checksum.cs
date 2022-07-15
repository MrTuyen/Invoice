using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DS.Common.Helpers
{
    public static class Checksum
    {
        public static string CreateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static string CreateMD5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                byte[] byteArray = md5.ComputeHash(stream);

                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(byteArray.Length * 2);
                foreach (byte byteMember in byteArray)
                {
                    stringBuilder.AppendFormat("{0:x2}", byteMember);
                }

                var check = stringBuilder.ToString();

                return check;
            }
        }
    }
}
