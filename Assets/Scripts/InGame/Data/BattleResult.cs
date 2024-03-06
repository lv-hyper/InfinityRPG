using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data
{
    public class BattleResult
    {
        bool battleSuccess;

        public BattleResult(bool battleSuccess)
        {
            this.battleSuccess = battleSuccess;
        }

        public bool IsSuccess(){return battleSuccess;}
    }
}
