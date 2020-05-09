using System;
using System.Collections.Generic;
using System.Linq;
using WiFiMapCore.Interfaces.Project;

namespace WiFiMapCore.Model.Project
{
    public class Project : IProject
    {
        public List<IScanPoint> ScanPoints = new List<IScanPoint>();
        public byte[] Bitmap { get; set; } = new byte[0];

        IEnumerable<IScanPoint> IProject.ScanPoints => ScanPoints;

        protected bool Equals(Project other)
        {
            return ScanPoints.SequenceEqual(other.ScanPoints) && Bitmap.SequenceEqual(other.Bitmap);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Project) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ScanPoints, Bitmap);
        }
    }
}