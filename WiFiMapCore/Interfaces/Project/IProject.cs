using System.Collections.Generic;

namespace WiFiMapCore.Interfaces.Project
{
    public interface IProject
    {
        byte[] Bitmap { get; }
        IEnumerable<IScanPoint> ScanPoints { get; }
    }
}