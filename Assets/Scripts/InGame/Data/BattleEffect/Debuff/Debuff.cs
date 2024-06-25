using InGame.Data.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.BattleEffect
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Battle Effect/Debuff")]
    public class Debuff : BattleEffect
    {

        public override string GetDescription()
        {
            string desc = "";
            //desc += String.Format("Buff - {0}\n", name);
            foreach (SkillAbility ability in abilities)
            {
                desc += String.Format("{0}\n",
                    ability.GetAbilityInString(level)
                );
            }
            desc += String.Format("Duration : {0}s\n", duration);
            return desc;
        }
    }
}
