using UnityEngine;

namespace RotationTypes
{
    [System.Serializable]
    public class AngleWithType
    {
        private AngleWithType()
        {
            
        }
        
        public AngleWithType(AngleType angleType, float angle, bool showAngleTypeSelector = false, bool showAllAngleTypes = false)
        {
            this.angleType = angleType;
            SetAngle(angle);
            this.showAngleTypeSelector = showAngleTypeSelector; 
            this.showAllAngleTypes = showAllAngleTypes;
        }
        
        [SerializeField] public bool showAllAngleTypes = false;
        [SerializeField] public bool showAngleTypeSelector = true; 
        [SerializeField] public AngleType angleType = AngleType.Radian;
        [SerializeField] private float circleParts = 1;

        public float GetAngle()
        {
            return AngleType.ConvertAngle(circleParts, AngleType.CirclePart, angleType); 
        }
        
        public void SetAngle(float angle)
        {
            circleParts = AngleType.ConvertAngle(angle, angleType, AngleType.CirclePart); 
        }

        public void SetAngle(AngleType angleType, float angle)
        {
            circleParts = AngleType.ConvertAngle(angle, angleType, AngleType.CirclePart); 
        }
    }
}