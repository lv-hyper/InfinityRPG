using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public class InstantConvertItemAction : IMobAction
    {
        [SerializeField] private Data.Item.Item targetItem, destItem;

        public override void Action(string triggerID)
        {
            IActionCondition condition = GetComponent<IActionCondition>();
            if (condition == null || condition.verify(triggerID))
            {
                Data.SaveData.GameConditionSaveDataManager.RemoveCondition(triggerID);

                Debug.Log("test");

                var targetItemStatus = Data.Item.ItemCollection.GetInstance().allCollection[targetItem.itemID];
                var destItemStatus = Data.Item.ItemCollection.GetInstance().allCollection[destItem.itemID];

                if (targetItemStatus.getCount() > 0 && destItemStatus.getCount() < destItemStatus.getMaxCount())
                {
                    Data.Item.ItemCollection.GetInstance().RemoveItemCount(targetItem.itemID);
                    Data.Item.ItemCollection.GetInstance().AddItemCount(destItem.itemID);
                    Data.SaveData.InventorySaveDataManager.SaveInventoryData();
                }

            }
        }
    }
}
