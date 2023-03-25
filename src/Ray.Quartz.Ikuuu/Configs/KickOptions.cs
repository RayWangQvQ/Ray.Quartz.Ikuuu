using System.Collections.Generic;

namespace Ray.Quartz.Ikuuu.Configs
{
    public class KickOptions
    {
        public bool Enable { get; set; }

        public long Tid { get; set; }

        public List<int> Floors { get; set; }

        public int IntervalSec { get; set; }

        public string Reply { get; set; }
    }
}
