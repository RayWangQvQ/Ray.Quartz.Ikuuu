using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Ray.Quartz.Ikuuu
{
    public class CookieManager
    {
        public int Index { get; set; }

        public Dictionary<int, AccountInfo> CookieContainerDic { get; set; } = new Dictionary<int, AccountInfo>();

        public void Init(List<AccountInfo> accounts)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                CookieContainerDic[i] = accounts[i];
            }
        }

        public AccountInfo CurrentAccount => CookieContainerDic[Index];
    }

    public class AccountInfo
    {
        public AccountInfo(string userName, string pwd, CookieContainer cookieContainer = null)
        {
            UserName = userName;
            Pwd = pwd;
            MyCookieContainer = cookieContainer ?? new CookieContainer();
        }

        public CookieContainer MyCookieContainer { get; set; }

        public string UserName { get; set; }

        public string Pwd { get; set; }

        public CookieContainer CloneCookieContainer()
        {
            return this.MyCookieContainer.ToJsonStr().JsonDeserialize<CookieContainer>();
        }
    }
}
