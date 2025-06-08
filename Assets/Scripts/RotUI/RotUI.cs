using UnityEngine;

namespace RotUI
{
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
