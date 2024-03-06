using InGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public class CharacterEquipmentCondition : IActionCondition
    {
        [SerializeField] Data.Item.Item item;
        [SerializeField] int count;
        string difficulty
        {
            get { return PlayerPrefs.GetString("difficulty", "normal"); }
        }

        public override bool verify(string triggerID)
        {
            if (Data.SaveData.GameConditionSaveDataManager.ContainsCondition(triggerID))
            {
                Debug.Log("*asdf*");
                return Data.SaveData.GameConditionSaveDataManager.GetCondition(triggerID);
            }

            if (Entity.Character.GetInstance() == null) return false;


            var _condition = Entity.Character.GetInstance().GetCurrentEquipmentSet().GetWornCount(item) >= count;
            Debug.Log("*asdf*"+_condition);

            Data.SaveData.GameConditionSaveDataManager.SetCondition(triggerID, _condition);
            return _condition;
        }
    }
}
