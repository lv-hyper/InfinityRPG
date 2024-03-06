using System.Collections.Generic;
using System.Numerics;

namespace InGame.Data.BattleInstance
{
    public abstract class AbstractInstance
    {
        public abstract BigInteger RollDamage();
        public abstract void Attack(AbstractInstance instance);
        public abstract void SkillAttack(AbstractInstance instance, Skill.Skill skill);
        public abstract void Damage(AbstractInstance instance, BigInteger amount, bool isCritical, string additionalInfo);
        public abstract void Recover(BigInteger damageAmount);
        public abstract void RegenerateHP(BigInteger amount);
        public abstract void RegenerateMana(BigInteger amount);
        public abstract void Buff(BattleEffect.Buff buff);
        public abstract void Debuff(BattleEffect.Debuff debuff);
        public abstract BigInteger GetMaxHealth();
        public abstract BigInteger GetMaxMana();
        public abstract bool GetCritical();
        public abstract float GetCriticalDamage();
        public abstract float GetTotalElementalDamageBonus(AbstractInstance instance);
        public abstract float GetElementalDamage(EnumElemental elemental);
        public abstract float GetElementalDefence(EnumElemental elemental);
        public abstract EnumEntityClass GetEntityClass();
        public abstract float GetDefenceRatio(AbstractInstance instance);
    }
}