using System;
using System.Collections.Generic;
using System.Linq;
using WiFIMap.Interfaces;

namespace WiFIMap.ViewModels
{
    public class ScanPoint
    {
        public ScanPoint(double left, double top, IEnumerable<IEntity> bssInfo)
        {
            Left = left;
            Top = top;
            BssInfo = bssInfo.ToArray();
        }

        public Double Left { get; set; }
        public Double Top { get; set; }
        public IEnumerable<IEntity> BssInfo { get; }
    }
}