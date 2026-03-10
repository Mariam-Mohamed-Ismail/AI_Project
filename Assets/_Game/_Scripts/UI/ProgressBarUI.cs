using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private GameObject barContainer;
        [SerializeField] private Image fillImage;

        public void Start()
        {
            fillImage.fillAmount = 1f;
            barContainer.SetActive(true);
        }

        public void SetAmount(float amount)
        {
            fillImage.fillAmount = Mathf.Clamp01(amount);
        }
    }
}
