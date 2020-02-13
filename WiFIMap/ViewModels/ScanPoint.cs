using System;

namespace WiFIMap.ViewModels
{
    public class ScanPoint
    {
        public ScanPoint(double left, double top)
        {
            Left = left;
            Top = top;
        }

        public Double Left { get; set; }
        public Double Top { get; set; }
    }
}