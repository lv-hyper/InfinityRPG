using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public class ItemThrowTrigger : MonoBehaviour
    {
        [SerializeField] string triggerID;
        [SerializeField] Data.Item.Item targetItem;
        void Awake()
        {
            var conditions = new List<IActionCondition>(GetComponents<IActionCondition>());
            if(Data.SaveData.GameConditionSaveDataManager.GetCondition(triggerID))
            {
                Action();
            }
        }

        bool ConditionMatches()
        {
            var conditions = new List<IActionCondition>(GetComponents<IActionCondition>());
            return conditions.Count == 0 || conditions.TrueForAll(x => x.verify(triggerID));
        }

        bool Verify()
        {
            var conditions = new List<IActionCondition>(GetComponents<IActionCondition>());

            if (!ConditionMatches())
            {
                return false;
            }

            else if (Data.SaveData.GameConditionSaveDataManager.ContainsCondition(triggerID))
                return Data.SaveData.GameConditionSaveDataManager.GetCondition(triggerID);

            else
                return true;
        }

        public void OnItemThrown(Data.Item.Item item)
        {
            if(Verify() && targetItem.itemID == item.itemID)
            {
                Data.SaveData.GameConditionSaveDataManager.SetCondition(triggerID, true);
                Action();
            }
            Debug.Log(Data.SaveData.GameConditionSaveDataManager.GetCondition(triggerID));
        }

        void Action()
        {
            var afterKillActions = GetComponents<IMobAction>();
            foreach(var e in afterKillActions)
            {
                e.Action(triggerID);
            }
        }
    }
}
