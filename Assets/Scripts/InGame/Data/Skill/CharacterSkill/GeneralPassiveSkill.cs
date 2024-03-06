using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/General Passive Skill")]
    public class GeneralPassiveSkill : PassiveSkill
    {
        public override bool activateCondition(Battle battle)
        {
            return true;
        }

        public override bool deactivateCondition(Battle battle)
        {
            return false;
        }
    }
}
