using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InGame.UI
{

    public class SetItemInformationController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] RectTransform content;
        public void OnEnable()
        {
            string information = Data.Item.Group.ItemSetCollection.GetInstance().GetItemSetEffectString(
                Entity.Character.GetInstance().GetCurrentEquipmentSet().GetItemList()
            );

            if(information == "")
            {
                information = "No Set Item Effect";
            }
            text.text = information;

            content.sizeDelta = new Vector2(1020, text.preferredHeight);

        }
    }
}
