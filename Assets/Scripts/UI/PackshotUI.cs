using UnityEngine;

namespace UI
{
    public class PackshotUI : MonoBehaviour
    {
        public GameObject Root;

        public void Show()
        {
            Root.SetActive(true);
        }

        public void OnInstallClick()
        {
#if UNITY_WEBGL
        PlayableSDK.OpenStore();
#endif
        }
    }
}