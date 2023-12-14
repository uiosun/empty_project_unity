using TMPro;
using UnityEngine;

namespace Index
{
    public class IndexPanel : MonoBehaviour
    {
        public TMP_Text version;
        public AudioClip goGameClip;

        private void Start()
        {
            UIManager.instance.RefreshNewScene(gameObject);
            version.text = GameManager.instance.config.ver.ToString();
        }

        // 开始游戏
        public void GoGame(bool needLoad)
        {
            SoundManager.instance.PlaySingle(goGameClip);
            
            if (needLoad)
            {
                needLoad = GameManager.instance.LoadGame();
            }

            if (!needLoad)
            {
                GameManager.instance.InitGame();
            }
            
            gameObject.SetActive(false);
            GameManager.instance.turnEvent.Invoke();
        }
    }
}