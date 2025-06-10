using UnityEngine;

public class OnClickScript : MonoBehaviour
{
    public Camera[] cams;
    [SerializeField] private bool simplifyToMainCamera = false; 
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camera camToRaycast; 
            if (simplifyToMainCamera)
            {
                camToRaycast = Camera.main;
            }
            else
            {
                int screenPartIndex = (int)((Input.mousePosition.x / Screen.width) * (cams.Length - 1));
                camToRaycast = cams[screenPartIndex];
            }
            
            Ray ray = camToRaycast.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, 100.0f, camToRaycast.cullingMask); 
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    Debug.Log($"Clicked {gameObject.name} (Raycast Hit)");
                }
            }
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red, 5.0f);
        }
    }
}
