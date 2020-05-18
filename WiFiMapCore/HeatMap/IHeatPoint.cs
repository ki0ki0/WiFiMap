using System.ComponentModel;

namespace WiFiMapCore.HeatMap
{
    public interface IHeatPoint
    {
        public int X { get; }
        public int Y { get; }
        public byte Intensity { get; }
    }
}