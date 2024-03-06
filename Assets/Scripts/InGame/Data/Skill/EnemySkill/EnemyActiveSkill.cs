using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{
    [Serializable]
    public abstract class EnemyActiveSkill : Skill
    {
        [SerializeField] protected long skillCoolDown;

        [SerializeField] protected bool isAttackSkill;

        [SerializeField] protected SkillType skillType;
        [SerializeField] protected EnumElemental elementalType;

        [SerializeField] protected float skillPercentDamage;

        [SerializeField] protected AnimationClip skillAnimation;
        public abstract void OnActive(BattleInstance.AbstractInstance target);
        public abstract void Attack(
            BattleInstance.AbstractInstance from,
            BattleInstance.AbstractInstance to
        );
        public abstract long GetSkillCoolDown();
        public abstract float GetSkillPercentDamage();

        public EnumElemental GetElementalType()
        {
            return elementalType;
        }

        protected override string skillID
        {
            get
            {
                return string.Format("{0}/{1}", "Enemy", name);
            }
        }

        public string GetSkillID() { return skillID; }
    }
}
