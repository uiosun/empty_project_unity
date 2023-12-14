using UnityEngine;
using UnityEngine.UI;

namespace _Common
{
    public class RemindPrefab : MonoBehaviour
    {
        public Text infoText;

        private float _cold;

        public void SetInfo(string info, float cold = 1.5f)
        {
            infoText.text = info;

            _cold = cold;
        }

        private void Update()
        {
            _cold -= Time.deltaTime;
            if (_cold <= 0)
            {
                OnClickClose();
            }
        }

        public void OnClickClose()
        {
            Destroy(gameObject);
        }
    }
}
