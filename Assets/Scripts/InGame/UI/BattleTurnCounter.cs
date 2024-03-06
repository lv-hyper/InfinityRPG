using InGame.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InGame.UI
{
    public class BattleTurnCounter : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] UnityEngine.UI.Image battleTurnIndicator;
        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Battle))
            { 
                Battle battle = (Battle)subject;

                battleTurnIndicator.fillAmount = 1 - (float)((double)battle.GetCurrentTurn() / (double)Battle.maxTurn);           
            }
        }
    }
}
