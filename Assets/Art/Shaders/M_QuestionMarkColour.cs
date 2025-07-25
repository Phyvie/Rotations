using UnityEngine;

public class M_QuestionMarkColour : MonoBehaviour
{
    [SerializeField] private M_VectorColour vectorColour;
    
    void Update()
    {
        Quaternion orientation = transform.rotation;
        vectorColour.Color =
            (orientation.w) * (orientation.w) * Color.white +
            (orientation.x) * (orientation.x) * Color.red +
            (orientation.y) * (orientation.y) *  Color.green +
            (orientation.z) * (orientation.z) * Color.blue; 

    }
}
