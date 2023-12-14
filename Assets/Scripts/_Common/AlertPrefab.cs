using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Common
{
    public class AlertPrefab : MonoBehaviour
    {
        public Text titleText;
        public Text infoText;
        public Button sureBtn;
        private Action _callbackSure = null;
        private Action _callbackClose = null;

        public void SetInfo(string title, string info, Action callbackSure = null, Action callbackClose = null)
        {
            titleText.text = title;
            infoText.text = info;
            _callbackClose = callbackClose;
            _callbackSure = callbackSure;

            if (_callbackSure == null)
            {
                sureBtn.gameObject.SetActive(false);
            }
        }

        public void OnClickSure()
        {
            _callbackSure?.Invoke();
            Destroy(gameObject);
        }

        public void OnClickClose()
        {
            _callbackClose?.Invoke();
            Destroy(gameObject);
        }
    }
}
