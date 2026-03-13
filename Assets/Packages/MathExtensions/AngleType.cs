using System;
using System.Collections.Generic;
using System.Linq;

namespace Packages.MathExtensions
{
    [Serializable]
    public enum EAngleType
    {
        Radian,
        Degree,
        CirclePart
    }

    public static class AngleType
    {
        #region Metadata Definition
        private struct Metadata
        {
            public string Name;
            public double Multiplier;
            public string Label;
        }

        private static readonly Dictionary<EAngleType, Metadata> _metadata = new Dictionary<EAngleType, Metadata>
        {
            { EAngleType.Radian, new Metadata { Name = "Radian", Multiplier = 2 * Math.PI, Label = "2PI" } },
            { EAngleType.Degree, new Metadata { Name = "Degree", Multiplier = 360, Label = "360°" } },
            { EAngleType.CirclePart, new Metadata { Name = "CirclePart", Multiplier = 1, Label = "Circle(s)" } }
        };
        #endregion Metadata Definition

        #region Public Static API
        public static readonly List<string> AngleTypeNames = _metadata.Values.Select(m => m.Name)
            .ToList();

        public static string GetName(this EAngleType type) => _metadata[type].Name;
        public static double GetMultiplier(this EAngleType type) => _metadata[type].Multiplier;
        public static string GetLabel(this EAngleType type) => _metadata[type].Label;

        public static int GetIndex(this EAngleType type) => (int)type;
        public static EAngleType FromIndex(int index) => (EAngleType)index;

        public static float ConvertAngle(float inAngle, EAngleType inAngleType, EAngleType outAngleType)
        {
            return (float)(inAngle / GetMultiplier(inAngleType) * GetMultiplier(outAngleType));
        }
        #endregion Public Static API
    }
}
