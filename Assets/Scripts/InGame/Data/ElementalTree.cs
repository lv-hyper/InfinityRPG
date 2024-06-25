using InGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InGame.Data.SaveData;
using Unity.VisualScripting;
using UnityEngine;

namespace InGame.Data
{
    [Serializable]
    public class ElementalTreeElement
    {
        [Serializable]
        public enum Category
        {
            None,
            Offensive,
            DamagePassive,
            Defensive,
            DefencePassive,
            Supportive
        }

        [Serializable]
        public struct Dependency
        {
            [SerializeField] public int tier, order;
        }

        [SerializeField] List<Dependency> dependency;
        [SerializeField] List<Dependency> requirement;
        [SerializeField] Category category;
        [SerializeField] Data.Skill.CharacterSkill skill;
        [SerializeField] int upgradeCount = 0;

        public Category GetCategory()
        {
            return category;
        }

        public List<Dependency> GetDependency()
        {
            return dependency;
        }

        public Data.Skill.CharacterSkill GetSkill()
        {
            return skill;
        }

        public int GetUpgradeCount()
        {
            return upgradeCount;
        }

        public void AddRequirement(Dependency req)
        {
            Debug.Log("Added: " + req.tier + " / " + req.order);
            requirement.Add(req);
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "New Elemental Tree", menuName = "ScriptableObject/Elemental Tree")]
    public class ElementalTree : ScriptableObject
    {
        [Serializable]
        public class Tier
        {
            [SerializeField] public float radius = 160, angleOffset = 0;
            [SerializeField] public List<ElementalTreeElement> elements;

            public ElementalTreeElement FindElement(int order)
            {
                return elements[order];
            }
        }

        [SerializeField] List<Tier> tiers;
        [SerializeField] EnumElemental elemental;
        [SerializeField] public Sprite mainIcon, offensiveSkillIcon, damagePassiveIcon, defensiveSkillIcon, defencePassiveIcon, utilitySkillIcon;
        [SerializeField] public Color orbitColor, lineCoilor;

        public EnumElemental GetElemental() { return elemental; }

        public int GetTierCount() { return tiers.Count; }

        public Tier GetTier(int tier)
        {
            if (tiers.Count <= tier) return null;
            if (tier < 0) return null;
            return tiers[tier];
        }
        
        public List<string> GetAvailableSkills(List<ElementalSoulSaveData.Perk> perks)
        {
            List<string> availableSkills = new List<string>();
            foreach (var perk in perks)
            {
                var element = GetTier(perk.tier)?.FindElement(perk.order);
                if (element != null)
                {
                    availableSkills.Add(element.GetSkill().GetSkillID());
                }
            }

            return availableSkills;
        }

        public bool IsSkillAvailable(string skillID)
        {
            foreach (var tier in tiers)
            {
                foreach (var element in tier.elements)
                {
                    if (element.GetSkill().GetSkillID() == skillID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetElementalTreeElementRequirement()
        {
            foreach (Tier tierList in tiers)
            {
                foreach (ElementalTreeElement element in tierList.elements)
                {
                    var dependencies = element.GetDependency();
                    foreach (var dep in dependencies)
                    {
                        GetTier(dep.tier).FindElement(dep.order).AddRequirement(dep);
                        Debug.Log(dep.tier + " / " + dep.order);
                    } 
                }
            }
        }
    }

    public class ElementalTreeCollection
    {
        private static ElementalTreeCollection instance;

        Dictionary<EnumElemental, ElementalTree> elementalTreeCollection;

        public static ElementalTreeCollection GetInstance()
        {
            if (instance == null)
                instance = new ElementalTreeCollection();

            return instance;
        }

        private ElementalTreeCollection()
        {
            elementalTreeCollection = new Dictionary<EnumElemental, ElementalTree>();
        }

        public void AddElementalTree(ElementalTree elementalTree)
        {
            elementalTreeCollection.Add(elementalTree.GetElemental(), elementalTree);
        }

        public ElementalTree GetElementalTree(EnumElemental elemental)
        {
            ElementalTree result = null;
            elementalTreeCollection.TryGetValue(elemental, out result);

            Debug.Assert(result != null);

            return result;
        }

        public void ResetCollection()
        {
            elementalTreeCollection.Clear();
        }

        public void SetElementalTreeElementRequirement()
        {
            Debug.Log(elementalTreeCollection.Count);
            foreach (var tree in elementalTreeCollection) 
            {
                tree.Value.SetElementalTreeElementRequirement();
            }
        }
    }
}
