using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public class InstantEncounterBossAction : IMobAction
    {
        [SerializeField] private Data.Mob.Boss boss;
        [SerializeField] private bool isRestartable = false;

        public override void Action(string triggerID)
        {
            IActionCondition condition = GetComponent<IActionCondition>();
            if (condition == null || condition.verify(triggerID))
            {
                Data.SaveData.GameConditionSaveDataManager.RemoveCondition(triggerID);

                if (!isRestartable)
                    Data.SaveData.GameConditionSaveDataManager.SetCondition(triggerID, false);

                var commonLayerGameObject = GameObject.Find("CommonLayer");

                if (commonLayerGameObject != null)
                {
                    foreach (Transform t in commonLayerGameObject.transform)
                    {
                        t.gameObject.SetActive(false);
                    }
                }

                if (GameObject.Find("MenuRoot") != null)
                {
                    GameObject.Find("MenuRoot").SetActive(false);
                }

                InGame.Action.BattleController.GetInstance().InitBattle(boss);
            }
        }
    }
}
