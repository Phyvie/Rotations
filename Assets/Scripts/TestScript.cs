using Extensions.MathExtensions;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private LockableFloat _lockableFloatA = new LockableFloat(1, false); 
    [SerializeField] private LockableFloat _lockableFloatB = new LockableFloat(0, false);
}
