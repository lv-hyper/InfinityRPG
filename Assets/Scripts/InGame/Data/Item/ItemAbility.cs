using UnityEngine;
using System;
using System.Collections.Generic;

namespace InGame.Data.Item
{
    /*
    public class ItemAbilityAttribute : Attribute
    {
        public ItemAbilityAttribute(
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
    public enum EnumItemAbility
    {
        [ItemAbility("atk", "Attack +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Attack,

        [ItemAbility("def", "Defence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Defence,

        [ItemAbility("str", "Strength +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Strength,

        [ItemAbility("int", "Intelligence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Intelligence,

        [ItemAbility("dex", "Dexterity +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Dexterity,

        [ItemAbility("vit", "Vitality +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Vitality,

        [ItemAbility("end", "Endurance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Endurance,

        [ItemAbility("allStat", "All Stat +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        AllStat,

        [ItemAbility("atk", "Attack +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)] 
        PercentPointAttack,

        [ItemAbility("def", "All Defence +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)] 
        PercentAllDefence,

        [ItemAbility("str", "Strength +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentPointStrength,

        [ItemAbility("int", "Intelligence +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentPointIntelligence,

        [ItemAbility("dex", "Dexterity +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentPointDexterity,

        [ItemAbility("vit", "Vitality +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentPointVitality,

        [ItemAbility("end", "Endurance +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentPointEndurance,

        [ItemAbility("allStat", "AllStat +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentPointAllStat,

        [ItemAbility("dropRate", "Droprate +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        PercentDroprate,

        [ItemAbility("goldRate", "Gold +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)] 
        PercentGold,

        [ItemAbility("speed", "Speed +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Speed,

        [ItemAbility("stamina", "Stamina +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        Stamina,

        [ItemAbility("stamina endurance", "Stamina Endurance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        StaminaEndurance,

        [ItemAbility("doubleHit", "Double Hit +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)] 
        PercentDoubleHit,

        [ItemAbility("criticalRate", "Critical Rate +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)] 
        PercentCritical,

        [ItemAbility("criticalDamage", "Critical Damage +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)] 
        PercentCriticalDamage,

        [ItemAbility("itemRecoverByDamage", "Recover By Damage +{0:N0}%", AbilityCalcStrategy.OverwriteGreaterCalcStrategy)] 
        PercentRecoverByDamage,

        [ItemAbility("itemRecoverByHPPercent", "Recover By HP +{0:N0}%", AbilityCalcStrategy.OverwriteGreaterCalcStrategy)] 
        PercentRecoverByHPPercent,

        [ItemAbility("expRate", "EXP +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)] 
        PercentEXP,

        [ItemAbility("movementSpeed", "Movement Speed +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        MovementSpeed,

        [ItemAbility("soulExtract", "Soul Extract", AbilityCalcStrategy.OverwriteGreaterCalcStrategy)] 
        SoulExtract,

        [ItemAbility("hp", "HP +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        HP,

        [ItemAbility("hp", "HP +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)] 
        PercentHP,

        [ItemAbility("additionalStatPoint", "Additional Stat Point +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)] 
        AdditionalStatPoint,

        [ItemAbility("totalDamageReduction", "Damage you take +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        TotalTakenDamage,

        [ItemAbility("additionalVitBasedOnSTR", "Additional Vitality Based on STR +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        AdditionalVitBasedOnSTR,

        [ItemAbility("additionalVitBasedOnINT", "Additional Vitality Based on INT +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        AdditionalVitBasedOnINT,

        [ItemAbility("additionalVitBasedOnDEX", "Additional Vitality Based on DEX +{0:N0}%p", AbilityCalcStrategy.GeneralCalcStrategy)]
        AdditionalVitBasedOnDEX,

        [ItemAbility("defenceExponential", "Defence Formula Exponential +{0:N0}%p", AbilityCalcStrategy.OverwriteGreaterCalcStrategy)]
        DefenceExponential,

        [ItemAbility("allStatByCollection", "All Stat +{0:N0} * Item Count with Stack", AbilityCalcStrategy.GeneralCalcStrategy)]
        AllStatByCollection,

        [ItemAbility("fireElemental", "Fire Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Fire,

        [ItemAbility("waterElemental", "Water Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Water,

        [ItemAbility("thunderElemental", "Thunder Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Thunder,

        [ItemAbility("windElemental", "Wind Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Wind,

        [ItemAbility("earthElemental", "Earth Elemental +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        Earth,

        [ItemAbility("fireResistance", "Fire Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        FireResistance,

        [ItemAbility("waterResistance", "Water Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        WaterResistance,

        [ItemAbility("thunderResistance", "Thunder Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        ThunderResistance,

        [ItemAbility("windResistance", "Wind Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        WindResistance,

        [ItemAbility("earthResistance", "Earth Resistance +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        EarthResistance,

        [ItemAbility("meleeDef", "Melee Defence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        MeleeDefence,

        [ItemAbility("meleeDef", "Melee Defence +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentMeleeDefence,

        [ItemAbility("magicalDef", "Magical Defence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        MagicalDefence,

        [ItemAbility("magicalDef", "Magical Defence +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentMagicalDefence,

        [ItemAbility("rangedDef", "Ranged Defence +{0:N0}", AbilityCalcStrategy.GeneralCalcStrategy)]
        RangedDefence,

        [ItemAbility("rangedDef", "Ranged Defence +{0:N0}%p", AbilityCalcStrategy.PercentPointCalcStrategy)]
        PercentRangedDefence,

        [ItemAbility("str", "Strength +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentStrength,

        [ItemAbility("int", "Intelligence +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentIntelligence,

        [ItemAbility("dex", "Dexterity +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentDexterity,

        [ItemAbility("vit", "Vitality +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentVitality,

        [ItemAbility("end", "Endurance +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentEndurance,

        [ItemAbility("allStat", "AllStat +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentAllStat,

        [ItemAbility("atk", "Attack +{0:N0}%", AbilityCalcStrategy.PercentCalcStrategy)]
        PercentAttack
    }
    */
    
    [Serializable]
    public struct ItemAbility
    {
        [SerializeField] public Ability.AbilityData abilityData;
        [SerializeField] bool isPassive;

        ItemAbility(Ability.AbilityData abilityData, bool isPassive)
        {
            this.abilityData = abilityData;
            this.isPassive = isPassive;
        }

        public void SetAbilityData(Ability.AbilityData abilityData)
        {
            this.abilityData = abilityData;
        }

        public string GetAbilityInString(){
            var amount = abilityData.GetRawAmount();
            if (amount == 0) return "";           

            string description = string.Format(abilityData.GetAbility().GetDescription(), amount);
            description = description.Replace("+-", "-");

            if (isPassive)
            {
                description = description + " (Passive)";
            }

            return description;
        }
        public string GetAbilityInStringExtended(double bonusAmount)
        {
            var amount = abilityData.GetRawAmount();

            if (amount == 0) return "";

            if (bonusAmount == 0) return GetAbilityInString();

            string description = string.Format(abilityData.GetAbility().GetDescription(),
                String.Format(
                    "{0:N0}(+{1:N0})",
                    amount, bonusAmount
                )
            );
            description = description.Replace("+-", "-");

            if (isPassive)
            {
                description = description + " (Passive)";
            }

            return description;
        }
        
        public double GetAmount(){return abilityData.GetRawAmount(); }
        //public double GetAmount(){return amount;}
        public bool IsPassive(){return isPassive;}
        public Ability.AbilityData GetAbility(){return abilityData;}

        /*
        public ItemAbilityAttribute GetAbilityInfo()
        {
            ItemAbilityAttribute abilityInfo =

           (ItemAbilityAttribute)Attribute.GetCustomAttribute(
               typeof(EnumItemAbility).GetField(
                   Enum.GetName(
                       typeof(EnumItemAbility),
                       abilityType
                   )
               ),
               typeof(ItemAbilityAttribute)
           );

            return abilityInfo;
        }

        public static ItemAbilityAttribute GetAbilityInfo(EnumItemAbility enumItemAbility)
        {
            ItemAbilityAttribute abilityInfo =

           (ItemAbilityAttribute)Attribute.GetCustomAttribute(
               typeof(EnumItemAbility).GetField(
                   Enum.GetName(
                       typeof(EnumItemAbility),
                       enumItemAbility
                   )
               ),
               typeof(ItemAbilityAttribute)
           );

            return abilityInfo;
        }
        */

        public static List<ItemAbility> AmpiflyAbility(List<string> targetAbility, List<ItemAbility> itemAbility, int stack)
        {
            List<ItemAbility> itemAbilities = new List<ItemAbility>();

            foreach(var ability in itemAbility)
            {
                ItemAbility abilityData = new ItemAbility(ability.abilityData, ability.isPassive);

                if (targetAbility.Contains(ability.GetAbility().GetAbility().name) && stack > 1)
                {
                    abilityData.SetAbilityData(new Ability.AbilityData(
                        abilityData.GetAbility().GetAbility(), abilityData.GetAmount() + (abilityData.GetAmount() * stack * 0.005)
                    ));
                }

                itemAbilities.Add(abilityData);
            }

            return itemAbilities;
        }
        public static List<ItemAbility> GetBonusAbility(List<string> targetAbility, List<ItemAbility> itemAbility, int stack)
        {
            List<ItemAbility> itemAbilities = new List<ItemAbility>();

            for (int i = 0; i < itemAbility.Count; ++i)
            {
                if (targetAbility.Contains(itemAbilities[i].GetAbility().GetAbility().name))
                {
                    var element = itemAbility[i];
                    element.abilityData.SetRawAmount(element.GetAmount() * stack / 200);

                    if (stack > 1)
                        itemAbilities.Add(element);
                }
            }

            return itemAbilities;
        }
    }
}