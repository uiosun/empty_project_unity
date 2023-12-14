using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Common
{
    public class MiniInputPrefab : MonoBehaviour
    {
        public Text titleText;
        public InputField valueInput;
        private Action<string> _callbackSure = null;
        private Action _callbackClose = null;

        public void SetInfo(string title, Action<string> callbackSure = null, Action callbackClose = null)
        {
            titleText.text = title;
            _callbackClose = callbackClose;
            _callbackSure = callbackSure;
        }

        public void OnClickSure()
        {
            if (valueInput.text == "")
            {
                // UIManager.instance.RemindNew(GameManager.instance.I18.GetText("138"));
                return;
            }
            _callbackSure?.Invoke(valueInput.text);
            Destroy(gameObject);
        }

        public void OnClickClose()
        {
            _callbackClose?.Invoke();
            Destroy(gameObject);
        }
    }
}
