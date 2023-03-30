using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Ray.Quartz.Ikuuu.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            CookieContainer ckContainer = new CookieContainer();

            var uri = new Uri("https://test.com");

            ckContainer.SetCookies(uri, "lang=zh-cn;");

            var str= ckContainer.GetCookieHeader(uri);
            Debug.WriteLine(str);

            Type type = ckContainer.GetType();
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var m = type.GetMethod("DomainTableCleanup", flags);
            m.Invoke(ckContainer, null);

            str = ckContainer.GetCookieHeader(uri);
            Debug.WriteLine(str);

            Debug.WriteLine(ckContainer.Count);
        }
    }
}