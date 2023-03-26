using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Ray.Quartz.Ikuuu
{
    public class CookieManager
    {
        public int Index { get; set; }

        public Dictionary<int, string> CookieStrDic { get; set; } = new Dictionary<int, string>();

        public string GetCurrentCookieStr()
        {
            var re = CookieStrDic[Index];

            if (re == null)
            {
                CookieStrDic[Index] = "";
                re = CookieStrDic[Index];
            }

            return re;
        }

        public void Add(int index, string ckStr)
        {
            CookieStrDic[index] = ckStr;
        }
    }
}
