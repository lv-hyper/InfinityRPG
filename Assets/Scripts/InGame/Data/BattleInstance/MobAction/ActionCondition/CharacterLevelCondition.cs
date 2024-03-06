using InGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public class CharacterLevelCondition : IActionCondition
    {
        [SerializeField] long level = -1;
        string difficulty
        {
            get { return PlayerPrefs.GetString("difficulty", "normal"); }
        }

        public override bool verify(string triggerID)
        {
            if (Data.SaveData.GameConditionSaveDataManager.ContainsCondition(triggerID))
            {
                return Data.SaveData.GameConditionSaveDataManager.GetCondition(triggerID);
            }

            if (Entity.Character.GetInstance() == null) return false;


            var _condition = Entity.Character.GetInstance().GetLevel() >= level && level != -1;
            Data.SaveData.GameConditionSaveDataManager.SetCondition(triggerID, _condition);
            return _condition;
        }
    }
}
