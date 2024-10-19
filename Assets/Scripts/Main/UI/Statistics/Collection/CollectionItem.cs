using TMPro;
using UnityEngine;

namespace Main.UI.Statistics.Collection
{
    public class CollectionItem : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Image enemyImage;
        [SerializeField] TextMeshProUGUI enemyTitle;

        [SerializeField] GameObject dropIcon;
        [SerializeField] Transform dropListTransform;

        Vector2 MatchImageSize(Vector2 originalImageSize, Vector2 constraintSize)
        {
            var imageWidth = originalImageSize.x;
            var imageHeight = originalImageSize.y;
            
            if(imageWidth > imageHeight)
            {
                var ratio = imageWidth / imageHeight;
                imageWidth = constraintSize.x;
                imageHeight = imageWidth / ratio;
            }
            else
            {
                var ratio = imageHeight / imageWidth;
                imageHeight = constraintSize.y;
                imageWidth = imageHeight / ratio;
            }

            return new Vector2(imageWidth, imageHeight);
        }

        public void Init(InGame.Data.Mob.AbstractMob mob, string mode)
        {
            var imageSize = new Vector2(
                mob.GetSprite().rect.width,
                mob.GetSprite().rect.height
            );
            
            Debug.Log(imageSize);

            imageSize = MatchImageSize(imageSize, new Vector2(128, 128));
            Debug.Log(imageSize);

            enemyImage.sprite = mob.GetSprite();
            enemyImage.rectTransform.sizeDelta = imageSize;

            var mobLevel = mob.GetLV();

            if (mob.IsBoss())
                enemyTitle.text = $"(Boss) Lv. {mobLevel:N0} {mob.name}";
            else
                enemyTitle.text = $"Max Lv. {mobLevel:N0} {mob.name}";


            var dropTable = mob.GetDropTable();
            
            for(var i=0; i<dropTable.Count;++i)
            {
                var drop = dropTable[i];
                
                var dropIconInstance = Instantiate(dropIcon, dropListTransform, false);
                var itemIcon = dropIconInstance.GetComponent<ItemIcon>();
                
                Debug.Log(drop.item.name);
                itemIcon.SetSprite(drop.item.GetSprite());

                var item = InGame.Data.Item.ItemCollection.GetInstance().
                    allCollection[drop.item.itemID];

                if (item.getCount() == item.getMaxCount())
                {
                    itemIcon.SetColor(Color.white);
                    itemIcon.SetOutlineColor(Color.yellow);
                }
                else if (item.getCount() == 0)
                {
                    itemIcon.SetColor(new Color(0.8f, 0.8f, 0.8f, 1));
                    itemIcon.SetOutlineColor(new Color(0.8f, 0.8f, 0.8f, 1));
                    itemIcon.SetGrayScaleMaterial(true);
                }

                var pos = dropIconInstance.GetComponent<RectTransform>().anchoredPosition;
                pos.x += (i * 100);
                dropIconInstance.GetComponent<RectTransform>().anchoredPosition = pos;
            }
        }
    }

}
