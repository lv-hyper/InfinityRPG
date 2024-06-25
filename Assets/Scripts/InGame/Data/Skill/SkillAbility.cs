using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{
    /*
    public class SkillAbilityAttribute : Attribute
    {
        public SkillAbilityAttribute(
            string abilityType,
            string abilityDescription, 
            AbilityCalcStrategy abilityCalcStrategy
        )
        {
            this.abilityType = abilityType;
            this.abilityDescription = abilityDescription;
            this.abilityCalcStrategy = abilityCalcStrategy;
        }
        public string abilityType { get; private set; }
        public string abilityDescription { get; private set; }
        public AbilityCalcStrategy abilityCalcStrategy { get; private set; }
    }

    [Serializable]
    public enum EnumSkillAbility
    {
        [SkillAbility("atk", "Attack +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Attack,

        [SkillAbility("def", "Defence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        AllDefence,

        [SkillAbility("atk", "Attack +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentAttack,

        [SkillAbility("def", "All Defence +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentAllDefence,

        [SkillAbility("dropRate", "Droprate +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        PercentDroprate,

        [SkillAbility("goldRate", "Gold +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        PercentGold,

        [SkillAbility("doubleHit", "Double Hit +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        PercentDoubleHit,

        [SkillAbility("criticalRate", "Critical Rate +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        PercentCritical,

        [SkillAbility("criticalDamage", "Critical Damage +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        PercentCriticalDamage,

        [SkillAbility("hp", "HP +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        HP,

        [SkillAbility("hp", "HP +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentHP,

        [SkillAbility("avoid", "Avoid +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        PercentAvoid,

        [SkillAbility("avoidAttack", "Chance to Additional Attack after Avoid +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        AvoidAdditionalDamage,

        [SkillAbility("reflectDamage", "Reflect Damage +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        Reflect,

        [SkillAbility("comboRate", "Combo rate +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        PercentCombo,

        [SkillAbility("fireElemental", "Fire Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Fire,

        [SkillAbility("waterElemental", "Water Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Water,

        [SkillAbility("thunderElemental", "Thunder Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Thunder,

        [SkillAbility("windElemental", "Wind Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Wind,

        [SkillAbility("earthElemental", "Earth Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Earth,

        [SkillAbility("fireResistance", "Fire Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        FireResistance,

        [SkillAbility("waterResistance", "Water Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        WaterResistance,

        [SkillAbility("thunderResistance", "Thunder Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        ThunderResistance,

        [SkillAbility("windResistance", "Wind Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        WindResistance,

        [SkillAbility("earthResistance", "Earth Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        EarthResistance,

        [SkillAbility("skillRecoverByDamage", "Recover by Damage +{0:N0}%", AbilityCalcStrategy.OverwriteGreaterCalcStrategy)]
        PercentRecoverByDamage,

        [SkillAbility("skillRecoverByHPPercent", "Recover by HP +{0:N0}%", AbilityCalcStrategy.OverwriteGreaterCalcStrategy)]
        PercentRecoverByHPPercent,

        [SkillAbility("defenceBreak", "Defence Break +{0:N0}%", AbilityCalcStrategy.GeneralCalcStrategy)]
        DefenceBreak,

        [SkillAbility("meleeDef", "Melee Defence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        MeleeDefence,

        [SkillAbility("meleeDef", "Melee Defence +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentMeleeDefence,

        [SkillAbility("magicalDef", "Magical Defence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        MagicalDefence,

        [SkillAbility("magicalDef", "Magical Defence +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentMagicalDefence,

        [SkillAbility("rangedDef", "Ranged Defence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        RangedDefence,

        [SkillAbility("rangedDef", "Ranged Defence +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentRangedDefence,
    }
    */
    [Serializable]
    public struct SkillAbility
    {
        [SerializeField] public Ability.AbilityData abilityData;
        [SerializeField] bool target;

        public SkillAbility(Ability.AbilityData _abilityData, bool _target)
        {
            abilityData = _abilityData;
            target = _target;
        }

        public string GetAbilityInString(long stack = 1)
        {
            var amount = abilityData.GetRawAmount();

            if (amount == 0) return "";

            double totalAmount = amount;

            //if (stack > 1) 
                totalAmount = stack * amount;

            string description = string.Format(abilityData.GetAbility().GetDescription(), totalAmount);
            description = description.Replace("+-", "-");

            return description;
        }

        public double GetAmount() { return abilityData.GetRawAmount(); }
        //public double GetAmount(){return amount;}

        public Ability.AbilityData GetAbility() { return abilityData; }

        public bool GetTarget() { return target; }

        /*
        public SkillAbilityAttribute GetAbilityInfo()
        {
            SkillAbilityAttribute abilityInfo =

            (SkillAbilityAttribute)Attribute.GetCustomAttribute(
                typeof(EnumSkillAbility).GetField(
                    Enum.GetName(
                        typeof(EnumSkillAbility),
                        abilityType
                    )
                ),
                typeof(SkillAbilityAttribute)
            );

            return abilityInfo;
        }

        public static SkillAbilityAttribute GetAbilityInfo(EnumSkillAbility enumSkillAbility)
        {
            SkillAbilityAttribute abilityInfo =

            (SkillAbilityAttribute)Attribute.GetCustomAttribute(
                typeof(EnumSkillAbility).GetField(
                    Enum.GetName(
                        typeof(EnumSkillAbility),
                        enumSkillAbility
                    )
                ),
                typeof(SkillAbilityAttribute)
            );

            return abilityInfo;
        }
        */
    }
}
