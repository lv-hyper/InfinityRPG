using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{
    [Serializable]
    public abstract class CharacterSkill : Skill
    {
        [SerializeField] protected EnumEntityClass targetCharacterClassType;
        [SerializeField] protected EnumElemental elementalType;
        [SerializeField] protected long upgradeCost;
        [SerializeField] protected long defaultUpgradeCount;
        [SerializeField] protected long maxUpgradeCount;
        [SerializeField] protected long requiredLevel;
        [SerializeField] protected string skillDescription;
        [SerializeField] protected bool isClassSkill;
        [SerializeField] protected override string skillID
        {
            get
            {
                return string.Format("{0}/{1}", targetCharacterClassType.ToString(), name);
            }
        }

        public EnumEntityClass GetTargetCharacterClassType(){ return targetCharacterClassType; }
        public virtual long GetUpgradeCost() { return upgradeCost; }
        public long GetDefaultUpgradeCount() {return defaultUpgradeCount; }
        public long GetMaxUpgradeCount() { return maxUpgradeCount; }
        public long GetRequiredLevel() {return requiredLevel; }
        public string GetSkillDescription() { return skillDescription; }
        public virtual string GetLongDescription(long stack) {
            return "";
        }
        public string GetSkillID() { return skillID; }
        public bool IsClassSkill() { return isClassSkill; }

        public virtual EnumElemental GetElementalType() { return elementalType; }
    }
}
