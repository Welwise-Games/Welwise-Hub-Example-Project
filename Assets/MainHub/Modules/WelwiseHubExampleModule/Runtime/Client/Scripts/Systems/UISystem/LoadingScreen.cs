using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Image _loadingBarImage;
        [SerializeField] private float _loadingDuration;

        public void Start()
        {
            StartCoroutine(FadeOut());
        }

        public void ApplyProgress(float value)
        {
            _progressText.text = ((int)(value * 100)) + "%";
            _progressSlider.value = value;
        }

        private IEnumerator FadeOut()
        {
            float loadingProgress = 0;
            while (loadingProgress < 1)
            {
                loadingProgress += 1 / _loadingDuration * Time.deltaTime;
                ApplyProgress(loadingProgress);

                yield return null;
            }


            yield return new WaitForSeconds(0.3f);

            gameObject.SetActive(false);
        }
    }
}