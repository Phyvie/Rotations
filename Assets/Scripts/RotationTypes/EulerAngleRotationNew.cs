using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class EulerAngleRotationNew
    {
        [SerializeField] public float ZyKa = -1; 
        
        [SerializeField] private GimbleRingNew firstGimbleRing; 
        [SerializeField] private GimbleRingNew secondGimbleRing; 
        [SerializeField] private GimbleRingNew thirdGimbleRing;
        private GimbleRingNew[] gimble;

        [SerializeField] public AngleType angleType = AngleType.Radian; 
        public AngleType AngleType
        {
            get => angleType;
            set
            {
                angleType = value;
                if (gimbleRingsInheritAngleType)
                {
                    foreach (GimbleRingNew gr in gimble)
                    {
                        gr.AngleType = value; 
                    }
                }
            }
        }
        [SerializeField] public bool gimbleRingsInheritAngleType = false;

        public EulerAngleRotationNew() : this(new GimbleRingNew(null), new GimbleRingNew(null), new GimbleRingNew(null))
        {
        }
        
        public EulerAngleRotationNew(GimbleRingNew firstGimbleRing, GimbleRingNew secondGimbleRing, GimbleRingNew thirdGimbleRing)
        {
            this.firstGimbleRing = firstGimbleRing; 
            this.secondGimbleRing = secondGimbleRing; 
            this.thirdGimbleRing = thirdGimbleRing;
            
            gimble = new[] { firstGimbleRing, secondGimbleRing, thirdGimbleRing };
            foreach (GimbleRingNew gr in gimble)
            {
                gr.eulerParent = this; 
            }
        }
    }

    [Serializable]
    public class GimbleRingNew
    {
        [SerializeReference] public EulerAngleRotationNew eulerParent;
        public bool bInheritsAngleType => eulerParent?.gimbleRingsInheritAngleType ?? false; 
        
        [SerializeField] private AngleType angleType = AngleType.Radian;
        public AngleType AngleType
        {
            get => angleType;
            set => angleType = value; 
        }

        public GimbleRingNew(EulerAngleRotationNew eulerParent)
        {
            this.eulerParent = eulerParent; 
        }
        
        private GimbleRingNew()
        {
            eulerParent = null; 
        }
    }
}