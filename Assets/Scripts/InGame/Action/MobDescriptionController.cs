using UnityEngine;

namespace InGame.Action
{
    public class MobDescriptionController : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI text;
        [SerializeField] UnityEngine.UI.Image image;

        public void Open(Data.Mob.AbstractMob mob)
        {
            gameObject.SetActive(true);
            text.text = mob.GetDescription();
            image.sprite = mob.GetSprite();
        }
    }
}