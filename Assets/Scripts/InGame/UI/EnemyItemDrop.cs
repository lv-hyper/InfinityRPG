using UnityEngine;
using Main.UI.Statistics;

namespace Ingame.UI
{
    public class EnemyItemDrop : MonoBehaviour
    {
        [SerializeField] GameObject dropIcon;
        [SerializeField] Transform dropListTransform;
        [SerializeField] string difficulty;
        [SerializeField] GameObject contentBox;

        public void Refresh(InGame.Data.Mob.AbstractMob mob)
        {
            foreach(Transform obj in dropListTransform)
            {
                Destroy(obj.gameObject);
            }

            float additionalDropRate = 
                (float)(100 + InGame.Data.Character.CharacterStat.GetAbilityAmount("Base Droprate")) / 100.0f;

            if (PlayerPrefs.GetString("passcode", "").ToLower() == "another")
                additionalDropRate /= 3.0f;

            int i = 0;
            foreach (var drop in mob.GetDropTable())
            {
                GameObject dropIconInstance = Instantiate(dropIcon, dropListTransform, false);
                ItemIcon itemIcon = dropIconInstance.GetComponent<ItemIcon>();
                
                itemIcon.SetSprite(drop.item.GetSprite());

                var item = InGame.Data.Item.ItemCollection.GetInstance().
                    allCollection[drop.item.itemID];

                var dropCount = InGame.Data.SaveData.DropTryCountSaveDataManager.GetDropTryCount(mob.name, i);
                Debug.Log("item number " + (i + 1) + ": " + dropCount);

                float dropMaxCountRatio = dropCount / (1.6f * (1 / (drop.probablity * additionalDropRate)));
                Debug.Log("item number " + (i + 1) + ": " + dropMaxCountRatio * 100 + "%");

                if (dropMaxCountRatio >= 1.0f)
                {
                    itemIcon.SetOutlineColor(new Color(0, 1, 0, 1));
                }
                
                else if (item.getCount() == item.getMaxCount())
                {
                    dropMaxCountRatio = 1.0f;
                    itemIcon.SetOutlineColor(new Color(1, 1, 0, 1));
                }

                if (item.getCount() == 0)
                {
                    itemIcon.SetGrayScaleMaterial(true);
                    itemIcon.SetColor(new Color(0.8f, 0.8f, 0.8f, 1));
                }
                
                else
                {
                    itemIcon.SetGrayScaleMaterial(false);
                }

                itemIcon.SetFillAmount(dropMaxCountRatio);


                var pos = dropIconInstance.GetComponent<RectTransform>().anchoredPosition;
                
                pos.x = i * 100 + 5;
                
                contentBox.transform.GetComponent<RectTransform>().sizeDelta = 
                    new UnityEngine.Vector2(i * 100 + 110, 90);
                
                dropIconInstance.GetComponent<RectTransform>().anchoredPosition = pos;

                ++i;
            }
        }
    }
}
