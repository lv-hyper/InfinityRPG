using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public class Character : AbstractInstance{
        Entity.Character character;

        BigInteger currentHealth;
        BigInteger currentMana;

        BigInteger recoverByHP, recoverByDamage;
        BigInteger currentDeathSurpass;

        bool isCritical;
        BigInteger critFailCount;

        public Character(Entity.Character character)
        {
            this.character = character;

            Item.Weapon weapon = (Item.Weapon)character.GetCurrentEquipmentSet().GetItem("weapon");

            currentHealth = (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Vitality") * 10;
            currentMana = (BigInteger)Data.Character.CharacterStat.GetMaxMana(character.GetLevel());

            isCritical = false;
            critFailCount = 0;

            recoverByHP = (BigInteger)Data.Character.CharacterStat.GetRecoverRateByHP();
            recoverByDamage = (BigInteger)Data.Character.CharacterStat.GetRecoverRateByDamage();

            currentDeathSurpass = (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Surpass Death");
        }

        public override void Attack(AbstractInstance instance)
        {

            bool isCritical = GetCritical();

            BigInteger baseAttack = (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Attack");
            BigInteger damageAmount = (BigInteger) ( (double)RollDamage() * GetTotalElementalDamageBonus(instance) );            

            if (isCritical)
            {
                var additionalCriticalDamage = Data.Character.CharacterStat.GetAbilityAmount("Critical Damage") / 100.0f;
                damageAmount = (BigInteger)((double)damageAmount * (Random.Range(0.9f, 1.1f)+ additionalCriticalDamage));
            }

            instance.Damage(this, (BigInteger)damageAmount, isCritical, "");
            Recover((BigInteger)((double)damageAmount / instance.GetDefenceRatio(this)));
        }     

        public override void SkillAttack(AbstractInstance instance, Skill.Skill skill)
        {
            Skill.ActiveSkill _skill = skill as Skill.ActiveSkill;

            long stack = Skill.SkillCollection.GetInstance().allSkillCollection[_skill.GetSkillID()].GetCount();
            currentMana -= _skill.GetManaCost(stack);

            if (currentMana < 0) currentMana = 0;
            if (currentMana > GetMaxMana()) currentMana = GetMaxMana();

            _skill.OnActive(this);
            _skill.Attack(this, instance);
        }

        public void ReflectAttack(AbstractInstance instance, float rate)
        {
            BigInteger baseAttack = (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Attack");
            BigInteger damageAmount = (BigInteger) ( (double)RollDamage() * GetTotalElementalDamageBonus(instance) * rate);           
            Recover(damageAmount);
            instance.Damage(this, damageAmount, false, "Reflect");
        }

        public void AvoidAttack(AbstractInstance instance)
        {
            BigInteger baseAttack = (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Attack");
            BigInteger damageAmount = (BigInteger) ( (double)RollDamage() * GetTotalElementalDamageBonus(instance));    
            Recover(damageAmount);
            instance.Damage(this, damageAmount, false, "Back-Attack");
        }

        public override void Recover(BigInteger damageAmount)
        {
            recoverByHP     = Data.Character.CharacterStat.GetRecoverRateByHP();
            recoverByDamage = Data.Character.CharacterStat.GetRecoverRateByDamage();

            currentHealth += GetMaxHealth() * recoverByHP / 100;
            currentHealth += damageAmount * recoverByDamage / 100;

            if (currentHealth > GetMaxHealth()) currentHealth = GetMaxHealth();
        }


        public override void Damage(AbstractInstance instance, BigInteger amount, bool isCritical, string additionalInfo)
        {
            if (Avoid(instance)) return;

            if(instance.GetType() == typeof(Mob))
            {
                Mob mob = (Mob)instance;

                float damageAdjustment = character.GetTakenDamageChanger();

                BigInteger mobLv = mob.GetLV();
                long resistCount = Entity.Character.GetInstance().GetResistCount(mob.GetTagList());

                BigInteger realDamage = (BigInteger)(
                    (double)amount / 
                    Data.Character.CharacterStat.GetDefenceRatio(mob.GetMobData()) 
                    * damageAdjustment
                );

                for(int i = 0; i < resistCount; i++)
                {
                    realDamage *= 7;
                    realDamage /= 10;
                }   

                currentHealth -= realDamage;

                if(currentHealth < 0)
                {
                    if(currentDeathSurpass > 0)
                    {
                        currentHealth = 1;
                        --currentDeathSurpass;
                    }
                }

                string damageText = "";

                if (additionalInfo != "")
                    damageText += string.Format("{0} ", additionalInfo);

                damageText = string.Format("{0}{1:N0}", damageText, realDamage);

                if (isCritical)
                    damageText = string.Format("*{0}*", damageText);

                UI.BattleDamageTextController.GetInstance().GenerateCharacterDamageText(damageText);

                float reflectDamageRate = (float)Data.Character.CharacterStat.GetAbilityAmount("Reflect Damage") / 100.0f;

                if (currentHealth > 0 && reflectDamageRate > 0)
                {
                    ReflectAttack(mob, reflectDamageRate);                        
                }
            }
        }

        public override BigInteger RollDamage()
        {
            BigInteger baseAttack = (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Attack");

            baseAttack = (BigInteger)((double)baseAttack); // * Random.Range(0.9f, 1.0f));

            return baseAttack;
        }

        public BigInteger GetHealth(){return currentHealth;}
        public override BigInteger GetMaxHealth(){return (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Vitality") * 10; }

        public BigInteger GetMana() { return currentMana; }
        public override BigInteger GetMaxMana() { return (BigInteger)Data.Character.CharacterStat.GetMaxMana(character.GetLevel()); }

        public bool isCombo() { return Random.Range(0, 1.0f) <= Data.Character.CharacterStat.GetAbilityAmount("Combo Rate") / 100.0; }

        void RollCritical() {
            float critMultiplier = 1.0f;
            float criticalRate = (float)Data.Character.CharacterStat.GetAbilityAmount("Critical Rate") / 100.0f;

            int maxFailCount = (int)Mathf.Ceil(1.6f / criticalRate);
            isCritical = Random.Range(0, 1.0f) <= criticalRate *critMultiplier;

            if(maxFailCount <= critFailCount) isCritical = true;

            if (!isCritical)
                critFailCount++;
            else
                critFailCount = 0;
        }

        public bool Avoid(AbstractInstance instance)
        {
            float avoidRate = (float)Data.Character.CharacterStat.GetAbilityAmount("Avoid Rate") / 100.0f;
            float avoidAttackRate = (float)Data.Character.CharacterStat.GetAbilityAmount("Avoid Damage") / 100.0f;
            bool isAvoid = Random.Range(0, 1.0f) <= avoidRate;

            if(isAvoid)
            {
                UI.BattleDamageTextController.GetInstance().GenerateCharacterDamageText("Avoid");
                if (avoidAttackRate >= (Random.Range(0, 1000000) / 1000000.0f))
                {
                    AvoidAttack(instance);
                }
            }
            return isAvoid;
        }
        public BigInteger GetDelayDeath() { return (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Delay Death") * 10; }

        public override bool GetCritical()
        {
            RollCritical();
            return isCritical;
        }

        public override float GetCriticalDamage()
        {
            float criticalDamage = (float)Data.Character.CharacterStat.GetAbilityAmount("Critical Damage") / 100.0f;
            return criticalDamage;
        }

        public override void Buff(BattleEffect.Buff buff)
        {
            BattleBuffController.GetInstance().AddCharacterBuff(buff, buff.duration);
        }

        public override void Debuff(BattleEffect.Debuff debuff)
        {
            BattleBuffController.GetInstance().AddCharacterDebuff(debuff, debuff.duration);
        }

        

        public override void RegenerateHP(BigInteger amount)
        {
            currentHealth += amount;
        }

        public override void RegenerateMana(BigInteger amount)
        {
            currentMana += amount;
        }

        public override EnumEntityClass GetEntityClass()
        {
            return character.GetCurrentClass();
        }
        public override float GetElementalDamage(EnumElemental elemental)
        {
            Dictionary<EnumElemental, string> elementalToRequiredStat = new Dictionary<EnumElemental, string> {
                { EnumElemental.None,       "" },
                { EnumElemental.Water,      "Base Water Damage" },
                { EnumElemental.Fire,       "Base Fire Damage" },
                { EnumElemental.Nature,     "Base Nature Damage" },
                { EnumElemental.Air,        "Base Air Damage" }
            };

            if (elementalToRequiredStat[elemental] == "") return 0;
            else return (float)Data.Character.CharacterStat.GetAbilityAmount(elementalToRequiredStat[elemental]);
        }

        public override float GetElementalDefence(EnumElemental elemental)
        {
            Dictionary<EnumElemental, string> elementalToRequiredStat = new Dictionary<EnumElemental, string> {
                { EnumElemental.None,       "" },
                { EnumElemental.Water,      "Base Water Defence" },
                { EnumElemental.Fire,       "Base Fire Defence" },
                { EnumElemental.Nature,     "Base Nature Defence" },
                { EnumElemental.Air,        "Base Air Defence" }
            };

            if (elementalToRequiredStat[elemental] == "") return 0;
            else return (float)Data.Character.CharacterStat.GetAbilityAmount(elementalToRequiredStat[elemental]);
        }

        public override float GetTotalElementalDamageBonus(AbstractInstance instance)
        {

            float sum = 0;
            for (var _elemental = EnumElemental.Air; _elemental <= EnumElemental.Count - 1; ++_elemental)
            {
                var diff = GetElementalDamage(_elemental) * (1 - (instance.GetElementalDefence(_elemental) / 100.0f));
                if (diff < 0) diff = 0;
                sum += diff;
            }

            float elementalBonus = sum / 100.0f;
            elementalBonus += 1;

            return elementalBonus;
        }

        public override float GetDefenceRatio(AbstractInstance instance)
        {
            if (instance.GetType() == typeof(Mob))
            {
                var _instance = (Mob)instance;
                return Data.Character.CharacterStat.GetDefenceRatio(
                    _instance.GetMobData()
                );
            }
            else return 1;
        }
    }
}