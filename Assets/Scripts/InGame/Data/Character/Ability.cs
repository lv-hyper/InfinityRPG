using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Character
{
    public class AbilityAttribute : Attribute
    {
        public AbilityAttribute(string abilityDescription, AbilityCalcStrategy abilityCalcStrategy)
        {
            this.abilityDescription = abilityDescription;
            this.abilityCalcStrategy = abilityCalcStrategy;
        }
        public string abilityDescription { get; private set; }
        public AbilityCalcStrategy abilityCalcStrategy { get; private set; }
    }

    [Serializable]
    public enum Ability
    {
        [Ability("Attack +{0:N0}",                  AbilityCalcStrategy.GeneralCalcStrategy)] Attack,
        [Ability("Defence +{0:N0}",                 AbilityCalcStrategy.GeneralCalcStrategy)] Defence,
        [Ability("Strength +{0:N0}",                AbilityCalcStrategy.GeneralCalcStrategy)] Strength, 
        [Ability("Intelligence +{0:N0}",            AbilityCalcStrategy.GeneralCalcStrategy)] Intelligence, 
        [Ability("Dexterity +{0:N0}",               AbilityCalcStrategy.GeneralCalcStrategy)] Dexterity, 
        [Ability("Vitality +{0:N0}",                AbilityCalcStrategy.GeneralCalcStrategy)] Vitality, 
        [Ability("Endurance +{0:N0}",               AbilityCalcStrategy.GeneralCalcStrategy)] Endurance, 
        [Ability("AllStat +{0:N0}",                 AbilityCalcStrategy.GeneralCalcStrategy)] AllStat,

        [Ability("Attack +{0:N0}%p",                AbilityCalcStrategy.PercentPointCalcStrategy)] PercentAttack, 
        [Ability("Defence +{0:N0}%p",               AbilityCalcStrategy.PercentPointCalcStrategy)] PercentDefence,

        [Ability("Strength +{0:N0}%",               AbilityCalcStrategy.PercentCalcStrategy)] PercentStrength, 
        [Ability("Intelligence +{0:N0}%",           AbilityCalcStrategy.PercentCalcStrategy)] PercentIntelligence, 
        [Ability("Dexterity +{0:N0}%",              AbilityCalcStrategy.PercentCalcStrategy)] PercentDexterity, 
        [Ability("Vitality +{0:N0}%",               AbilityCalcStrategy.PercentCalcStrategy)] PercentVitality, 
        [Ability("Endurance +{0:N0}%",              AbilityCalcStrategy.PercentCalcStrategy)] PercentEndurance, 
        [Ability("AllStat +{0:N0}%",                AbilityCalcStrategy.PercentCalcStrategy)] PercentAllStat,

        [Ability("Droprate +{0:N0}%p",              AbilityCalcStrategy.PercentPointCalcStrategy)] PercentDroprate, 
        [Ability("Gold +{0:N0}%p",                  AbilityCalcStrategy.PercentPointCalcStrategy)] PercentGold,

        [Ability("Speed +{0:N0}",                   AbilityCalcStrategy.GeneralCalcStrategy)] Speed, 
        [Ability("Stamina +{0:N0}",                 AbilityCalcStrategy.GeneralCalcStrategy)] Stamina, 
        [Ability("Stamina Endurance +{0:N0}",        AbilityCalcStrategy.GeneralCalcStrategy)] StaminaEndurance,

        [Ability("Double Hit +{0:N0}%p",            AbilityCalcStrategy.PercentPointCalcStrategy)] PercentDoubleHit, 
        [Ability("Critical Rate +{0:N0}%p",         AbilityCalcStrategy.PercentPointCalcStrategy)] PercentCritical, 
        [Ability("Critical Damage +{0:N0}%p",       AbilityCalcStrategy.PercentPointCalcStrategy)] PercentCriticalDamage,

        [Ability("Recover by Damage +{0:N0}%",      AbilityCalcStrategy.OverwriteGreaterCalcStrategy)] PercentRecoverByDamage, 
        [Ability("Recover by HP +{0:N0}%",          AbilityCalcStrategy.OverwriteGreaterCalcStrategy)] PercentRecoverByHPPercent,

        [Ability("EXP +{0:N0}%p",                   AbilityCalcStrategy.PercentPointCalcStrategy)] PercentEXP,

        [Ability("Movement Speed +{0:N0}",          AbilityCalcStrategy.GeneralCalcStrategy)] MovementSpeed,

        [Ability("Soul Extract",                    AbilityCalcStrategy.OverwriteGreaterCalcStrategy)] SoulExtract,

        [Ability("HP +{0:N0}",                      AbilityCalcStrategy.GeneralCalcStrategy)] HP, 
        [Ability("HP +{0:N0}%p",                    AbilityCalcStrategy.PercentPointCalcStrategy)] PercentHP,

        [Ability("Additional Stat Point +{0:N0}",   AbilityCalcStrategy.GeneralCalcStrategy)] AdditionalStatPoint
    }
}
