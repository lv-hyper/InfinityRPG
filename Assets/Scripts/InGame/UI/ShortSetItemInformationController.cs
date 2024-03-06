using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InGame.UI
{

    public class ShortSetItemInformationController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        public void Refresh()
        {
            string information = Data.Item.Group.ItemSetCollection.GetInstance().GetItemSetName(
                Entity.Character.GetInstance().GetCurrentEquipmentSet().GetItemList()
            );

            if(information == "")
            {
                information = "No Set Item Effect";
            }
            GetComponent<TextMeshProUGUI>().text = information;
        }
    }
}
