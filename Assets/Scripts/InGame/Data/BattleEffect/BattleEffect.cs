using InGame.Data.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InGame.Data.Ability;
using UnityEngine;

namespace InGame.Data.BattleEffect
{
    public abstract class BattleEffect : ScriptableObject
    {
        [SerializeField] Sprite effectIcon;
        [SerializeField] public int duration;
        [SerializeField] public int level = 1;
        [SerializeField] public List<SkillAbility> abilities;

        public Sprite GetIcon() { return effectIcon; }

        public List<SkillAbility> GetAbilities()
        {
            if (level == 1) return abilities;

            else
            {
                List<SkillAbility> result = new List<SkillAbility>();

                foreach (var _ability in abilities)
                {
                    var abilityData = _ability.GetAbility();
                    result.Add(new SkillAbility(abilityData * level, _ability.GetTarget()));
                }

                return result;
            }

        }

        public abstract String GetDescription();
    }
}
