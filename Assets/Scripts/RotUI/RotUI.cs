using UnityEngine;

namespace RotUI
{
    //-ZyKa RotUI -ZyKa Cleanup figure out whether you actually need this class, as well as all the other RotUI classes; because I think they are all just managed via the UXML & Binding
    public class RotUI<TRotParams> : MonoBehaviour
    {
        private TRotParams _rotParams;

        public RotUI(TRotParams rotParams)
        {
            RotParams = rotParams;
        }

        public TRotParams RotParams
        {
            get => _rotParams;
            set => _rotParams = value;
        }
    }
}
