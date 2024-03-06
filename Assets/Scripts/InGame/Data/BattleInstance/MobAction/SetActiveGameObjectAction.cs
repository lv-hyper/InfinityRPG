using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public class SetActiveGameObjectAction: IMobAction
    {
        [SerializeField] private List<GameObject> gameObjectToActivate;
        [SerializeField] private List<GameObject> gameObjectToDeactivate;

        public override void Action(string triggerID)
        {
            IActionCondition condition = GetComponent<IActionCondition>();
            if(condition == null || condition.verify(triggerID))
            {
                foreach(GameObject gameObject in gameObjectToActivate)
                {
                    gameObject.SetActive(true);
                }
                foreach (GameObject gameObject in gameObjectToDeactivate)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
