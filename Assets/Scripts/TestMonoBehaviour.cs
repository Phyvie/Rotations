using System;
using System.Collections.Generic;
using RotationTypes;
using UnityEngine;

public class TestMonoBehaviour : MonoBehaviour
{
    [SerializeField] private GimbleRing gimbleRing; 
    [SerializeField] private GimbleRing[] gimbleArray;
    [SerializeField] private List<GimbleRing> gimbleList;
}
