using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PackshotUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup rootCanvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Button button;

        private void Awake()
        {
            if (rootCanvasGroup != null)
            {
                rootCanvasGroup.alpha = 0f;
                rootCanvasGroup.gameObject.SetActive(false);
            }

            button.onClick.AddListener(OnPlayNowClick);
        }


        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnPlayNowClick);
        }

        public void Show()
        {
            StartCoroutine(ShowSequence());
        }

        private IEnumerator ShowSequence()
        {
            if (rootCanvasGroup != null)
            {
                rootCanvasGroup.gameObject.SetActive(true);
                rootCanvasGroup.alpha = 0f;

                rootCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad);
                yield return new WaitForSeconds(fadeDuration);
            }
        }

        public void OnPlayNowClick()
        {
#if UNITY_WEBGL
            Luna.Unity.LifeCycle.GameEnded();
            Luna.Unity.Playable.InstallFullGame();
#endif
        }
    }
}