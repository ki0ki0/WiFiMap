namespace WiFIMap.HeatMap
{
	public struct HeatPoint
	{
		public int X;
		public int Y;
		public byte Intensity;

		public HeatPoint(int x, int y, byte intensity)
		{
			X = x;
			Y = y;
			Intensity = intensity;
		}
	}
}
