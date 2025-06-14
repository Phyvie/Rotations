using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;

namespace RotParams
{
    [System.Serializable]
    public class AngleWithType : INotifyPropertyChanged
    {
        public static double PI_d = 3.141592653589793238462643383279; 
        
        [SerializeField] public bool showAllAngleTypes = false;
        [SerializeField] public bool showAngleTypeSelector = true;
        [SerializeReference] public AngleType angleType = AngleType.Radian;
        [SerializeField] private double _angleInRadian = 0f;

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
            get => (float) _angleInRadian;
            set
            {
                _angleInRadian = value;
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AngleInDegree
        {
            get => (float) (_angleInRadian * Mathf.Rad2Deg);
            set
            {
                _angleInRadian = value * Mathf.Deg2Rad;
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AngleInCircleParts
        {
            get => (float) (_angleInRadian / (2 * PI_d));
            set
            {
                _angleInRadian = value * 2 * PI_d;
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AngleInCurrentUnit
        {
            get => (float) (_angleInRadian * (angleType?.UnitMultiplier ?? 1.0) / (2 * PI_d));
            set
            {
                _angleInRadian = (float)(value / (angleType?.UnitMultiplier ?? 1.0) * 2 * PI_d);
                OnPropertyChanged();
            }
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
            return Mathf.Approximately((float) a._angleInRadian, (float) b._angleInRadian);
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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion INotifyPropertyChanged
    }
}