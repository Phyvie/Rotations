using UnityEngine;

public class CodeScreenshots : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; 
    [SerializeField] 
    private Quaternion targetRotation = 
        new Quaternion(0.6f, 0, 0, 0.8f);
    
    void Update()
    {
        Quaternion targetRotation = new Quaternion();

        transform.eulerAngles = new Vector3(0, -90, 0);
        
        transform.rotation = 
            Quaternion.Euler(new Vector3(0, -90, 0));
        transform.rotation = 
            new Quaternion(0.6f, 0, 0, 0.8f);
        
        Vector3 forwardVector = Vector3.up;
        Vector3 upVector = Vector3.forward; 
            
        transform.rotation = 
            Quaternion.LookRotation(
                forwardVector, upVector);
    }
}
