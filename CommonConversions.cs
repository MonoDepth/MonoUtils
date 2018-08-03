using System;
using System.Net.Http;
using System.Collections.Generic;
namespace MonoUtilities.Conversion
    {
        static class Conversion
        {
            public static DateTime FromUnixTime(this long unixTime)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(unixTime);
            }

            public static bool IsInt(this string s)
            {
                return int.TryParse(s, out int x);
            }
            public static bool IsInt(this char c)
            {
                string s = c.ToString();
                return int.TryParse(s, out int x);
            }
            public static FormUrlEncodedContent KeyPairsToHttpContent(List<KeyValuePair<string, string>> keyPair)
            {
                return new FormUrlEncodedContent(keyPair);
            }
         }
    }