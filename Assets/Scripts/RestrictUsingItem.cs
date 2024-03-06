using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictUsingItem : MonoBehaviour
{
    [SerializeField] InGame.Data.Item.Item item;
    [SerializeField] UnityEngine.UI.Button button;
    private void Awake()
    {
        bool isItemExist = false;
        if (InGame.Data.Item.ItemCollection.GetInstance().allCollection.ContainsKey(item.itemID))
        {
            isItemExist = InGame.Data.Item.ItemCollection.GetInstance().allCollection[item.itemID].getCount() > 0;
        }

        if(!isItemExist)
            button.interactable = false;
        else
            button.interactable = true;

    }
}
