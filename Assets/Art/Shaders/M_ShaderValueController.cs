using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Art.Shaders
{
    public class M_ShaderValueController : MonoBehaviour
    {
        [SerializeField] private Material editSharedMaterial;

        private Material runtimeMaterialInstanceCache;
        private Material runtimeMaterialInstance => runtimeMaterialInstanceCache ?? (runtimeMaterialInstanceCache = new Material(editSharedMaterial)); 
        public Material material => Application.isPlaying ? runtimeMaterialInstance : editSharedMaterial;

        protected virtual void Awake()
        {
            FindRuntimeMaterial(); 
        }

        protected virtual void OnDestroy()
        {
            runtimeMaterialInstanceCache = null;
        }

        protected void FindRuntimeMaterial()
        {
            Renderer renderer = GetComponent<Renderer>();
            runtimeMaterialInstanceCache = renderer.materials.FirstOrDefault(mat => mat == editSharedMaterial);
            runtimeMaterialInstanceCache = renderer.material; 
            runtimeMaterialInstanceCache.name = Regex.Replace(runtimeMaterialInstanceCache.name, @"\([^)]*\)", $"{gameObject.name}");
        }
    }
}