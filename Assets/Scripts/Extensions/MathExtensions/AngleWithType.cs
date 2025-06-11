using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace RotParams
{
    [System.Serializable]
    public class AngleWithType
    {
        [SerializeField] public bool showAllAngleTypes = false;
        [SerializeField] public bool showAngleTypeSelector = true;
        [SerializeReference] public AngleType angleType = AngleType.Radian;
        [SerializeField] private float _angleInRadian = 0f;

        #region Constructors
        private AngleWithType() { }

        public AngleWithType(AngleType angleType, float valueInAngleType, bool showAngleTypeSelector = true, bool showAllAngleTypes = false)
        {
            this.angleType = angleType;
            this.showAngleTypeSelector = showAngleTypeSelector;
            this.showAllAngleTypes = showAllAngleTypes;
            this.AngleInCurrentUnit = valueInAngleType;
        }

        #endregion Constructors

        #region Properties

        [CreateProperty]
        public float AngleInRadian
        {
            get => _angleInRadian;
            set => _angleInRadian = value;
        }

        [CreateProperty]
        public float AngleInDegree
        {
            get => _angleInRadian * Mathf.Rad2Deg;
            set => _angleInRadian = value * Mathf.Deg2Rad;
        }

        [CreateProperty]
        public float AngleInCircleParts
        {
            get => _angleInRadian / (2 * Mathf.PI);
            set => _angleInRadian = value * 2 * Mathf.PI;
        }

        [CreateProperty]
        public float AngleInCurrentUnit
        {
            get => (float)(_angleInRadian * (angleType?.UnitMultiplier ?? 1.0) / (2 * Mathf.PI));
            set => _angleInRadian = (float)(value / (angleType?.UnitMultiplier ?? 1.0) * 2 * Mathf.PI);
        }

        #endregion Properties

        #region Operator Overloads

        public static implicit operator float(AngleWithType angle) => angle.AngleInCurrentUnit;

        public static AngleWithType operator +(AngleWithType a, float b)
        {
            return new AngleWithType(a.angleType, a.AngleInCurrentUnit + b);
        }

        public static AngleWithType operator -(AngleWithType a, float b)
        {
            return new AngleWithType(a.angleType, a.AngleInCurrentUnit - b);
        }

        public static AngleWithType operator *(AngleWithType a, float b)
        {
            return new AngleWithType(a.angleType, a.AngleInCurrentUnit * b);
        }

        public static AngleWithType operator /(AngleWithType a, float b)
        {
            return new AngleWithType(a.angleType, a.AngleInCurrentUnit / b);
        }

        public static bool operator ==(AngleWithType a, AngleWithType b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return Mathf.Approximately(a._angleInRadian, b._angleInRadian);
        }

        public static bool operator !=(AngleWithType a, AngleWithType b) => !(a == b);

        #endregion Operator Overloads

        #region Overrides

        public override bool Equals(object obj)
        {
            return obj is AngleWithType other && this == other;
        }

        public override int GetHashCode()
        {
            return _angleInRadian.GetHashCode();
        }

        public override string ToString()
        {
            return $"{AngleInCurrentUnit:F2} {angleType?.UnitLabel}";
        }
        #endregion Overrides
        
        #region UI Toolkit Binding Support
        // Expose the angle types as choice strings for DropdownField.choices
        [CreateProperty]
        public List<string> AngleTypeChoices => AngleType.AngleTypeNames;

        // Index of current type in the list
        [CreateProperty]
        public int AngleTypeIndex
        {
            get => Array.IndexOf(AngleType.AngleTypes, angleType);
            set
            {
                if (value >= 0 && value < AngleType.AngleTypes.Length)
                    angleType = AngleType.AngleTypes[value];
            }
        }
        #endregion
    }
}