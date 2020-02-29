using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WiFIMap.Interfaces;

namespace WiFIMap.ViewModels
{
    public class ScanPoint
    {
        public ScanPoint(int left, int top, IEnumerable<IEntity> bssInfo)
        {
            Left = left;
            Top = top;
            BssInfo = bssInfo.ToArray();
        }

        public ScanPoint(int left, int top)
        {
            Left = left;
            Top = top;
        }

        public ScanPoint()
        {
        }

        public int Left { get; set; }
        public int Top { get; set; }
        public IEnumerable<IEntity> BssInfo { get; set; }
    }
}