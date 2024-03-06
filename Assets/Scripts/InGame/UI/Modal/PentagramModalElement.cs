using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using InGame.Data.Item;

namespace InGame.UI
{
    public class PentagramModalElement : MonoBehaviour
    {
        [SerializeField] Image itemImage;
        Item item_saved;
        [SerializeField] TMPro.TextMeshProUGUI itemRequirementText;
        [SerializeField] Transform descriptionAreaTransform;
        [SerializeField] GameObject descriptionArea;
        [SerializeField] GameObject screenPrefab; 

        public void InitElement(Item item, int currentCount, int maxCount)
        {
            itemImage.sprite = item.itemSprite;
            item_saved = item;
            itemRequirementText.text = string.Format("{0}/{1}",currentCount,maxCount);
        }

        public void ShowDescription()
        {
            descriptionAreaTransform = GameObject.Find("DescriptionArea").transform;
            
            foreach(Transform obj in descriptionAreaTransform)
            {
                Destroy(obj.gameObject);
            }
            
            GameObject descriptionScreen = Instantiate(screenPrefab);
            
            descriptionScreen.transform.SetParent(descriptionAreaTransform, false);
            descriptionScreen.transform.localScale = Vector3.one;

            descriptionScreen.GetComponent<MaterialDescriptionController>().Reload(item_saved);

            descriptionScreen.SetActive(true);
        }
    }
}
