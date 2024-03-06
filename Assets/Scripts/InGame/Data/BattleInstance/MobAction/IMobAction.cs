using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public abstract class IMobAction : MonoBehaviour
    {
        public abstract void Action(string triggerID);
    }
}
