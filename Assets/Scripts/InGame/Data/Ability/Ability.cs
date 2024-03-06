using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    public abstract class Ability : ScriptableObject
    {
        [SerializeField] string desc;
        public virtual AbilityData AggregateAbility(AbilityData originalAbility, AbilityData newAbility)
        {
            double amount = originalAbility.GetRawAmount() + newAbility.GetRawAmount();

            AbilityData ability = new AbilityData(
                originalAbility.GetAbility(),
                amount
            );

            return ability;
        }

        public virtual double GetAmount(AbilitySet abilitySet, double amount = 0)
        {
            return amount;
        }

        public virtual string GetDescription()
        {
            return desc;
        }

        public virtual double GetInitialAmount()
        {
            return 0;
        }

    }

    [Serializable]
    public class AbilityData
    {
        [SerializeField] Ability ability;
        [SerializeField] double amount;
        
        public AbilityData(Ability ability, double amount)
        {
            this.ability = ability;
            this.amount = amount;
        }

        public Ability GetAbility()
        {
            return ability;
        }

        public double GetAmount(AbilitySet abilitySet)
        {
            return ability.GetAmount(abilitySet, amount);
        }

        public double GetRawAmount()
        {
            return amount;
        }

        public void SetRawAmount(double amount)
        {
            this.amount = amount;
        }
    }

    public class AbilitySet
    {
        List<AbilityData> abilities;
        Dictionary<string, AbilityData> abilityDict;

        public AbilitySet()
        {
            abilities = new List<AbilityData>();
            abilityDict = new Dictionary<string, AbilityData>();
        }

        public void SetAbility(List<AbilityData> abilities) { 
            this.abilities = abilities;
            RefreshDict();
        }

        public void AddAbility(AbilityData ability)
        {
            abilities.Add(ability);

            if (abilityDict.ContainsKey(ability.GetAbility().name))
            {
                Ability _ability = abilityDict[ability.GetAbility().name].GetAbility();
                abilityDict[ability.GetAbility().name] =
                    _ability.AggregateAbility(abilityDict[ability.GetAbility().name], ability);
            }
            else
            {
                abilityDict[ability.GetAbility().name] = ability;
            }
        }

        public void ClearAbility()
        {
            abilities.Clear();
            RefreshDict();
        }

        public void RefreshDict()
        {
            if (abilities.Count <= 0) return;
            abilityDict.Clear();

            var groupedAbility = abilities.GroupBy(x => x.GetAbility().name);

            foreach (var group in groupedAbility)
            {
                abilityDict.Add(group.Key, group.ToList().Aggregate(
                    (originalAbility, newAbility) => {
                        return originalAbility.GetAbility().AggregateAbility(originalAbility, newAbility);
                    }
                ));
            }
        }

        public Dictionary<string, AbilityData> GetAbilities()
        {
            return abilityDict;
        }

        public AbilityData GetAbility(string abilityStr)
        {
            if(abilityDict.ContainsKey(abilityStr))
                return abilityDict[abilityStr];
            return null;
        }

        public double GetAbilityAmount(string abilityStr)
        {
            var _ability = GetAbility(abilityStr);
            if (_ability == null)
            {
                var ability = AbilityCollection.GetInstance().GetAbility(abilityStr);
                Debug.AssertFormat(ability != null, "abilityStr = {0}", abilityStr);
                return ability.GetAmount(this, ability.GetInitialAmount());
            }
            else
            {
                return _ability.GetAmount(this);
            }
        }
    }

    public class AbilityCollection
    {
        private static AbilityCollection instance;
        public Dictionary<string, Ability> abilities;

        public static AbilityCollection GetInstance()
        {
            if (instance == null)
            {
                instance = new AbilityCollection();
            }

            return instance;
        }

        AbilityCollection()
        {
            abilities = new Dictionary<string, Ability>();
        }

        public void AddAbility(Ability ability)
        {
            abilities.Add(ability.name, ability);
        }

        public Ability GetAbility(string abilityStr)
        {
            if (abilities.ContainsKey(abilityStr))
                return abilities[abilityStr];
            else return null;
        }

        public void ResetAbility()
        {
            abilities.Clear();
        }
    }
}
