namespace WiFiMapCore.HeatMap
{
    public struct HeatPoint
    {
        public int X { get; }
        public int Y { get; }
        public byte Intensity { get; }

        public HeatPoint(int x, int y, byte intensity)
        {
            X = x;
            Y = y;
            Intensity = intensity;
        }
    }
}