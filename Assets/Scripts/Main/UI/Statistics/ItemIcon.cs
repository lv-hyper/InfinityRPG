using UnityEngine;
using UnityEngine.UI;

namespace Main.UI.Statistics
{
    public class ItemIcon : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image fgImage, itemImage;
        [SerializeField] Material GrayScaleMaterial, BaseMaterial;

        public void SetSprite(Sprite sprite)
        {
            itemImage.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            itemImage.color = color;
        }

        public void SetFillAmount(float amount)
        {
            fgImage.GetComponent<UnityEngine.UI.Image>().fillAmount = amount;
        }

        public void SetOutlineColor(Color color)
        {
            fgImage.GetComponent<Outline>().effectColor = color;
        }

        public void SetGrayScaleMaterial(bool isGrayScale)
        {
            if (isGrayScale)
            {
                itemImage.material = GrayScaleMaterial;
            }
            else
            {
                itemImage.material = BaseMaterial;
            }
        }
    }
}