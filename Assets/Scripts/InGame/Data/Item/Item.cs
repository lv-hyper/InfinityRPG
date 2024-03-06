using UnityEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using System.Numerics;
using System.Runtime.InteropServices;
using InGame.Data.Item.Group;

namespace InGame.Data.Item
{

    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item/Item")]
    public class Item : ScriptableObject
    {
        [SerializeField] public Sprite itemSprite;
        [SerializeField] public ItemCategory.Zone zone;
        [SerializeField] public string legacyID;
        [SerializeField] public long level = 0;
        [SerializeField] public int order = 0;
        [SerializeField] public int maxStack = 1;
        [SerializeField] public int maxWornCount = 1;
        [SerializeField] public long price = 0;
        [SerializeField] public string shortDesc;
        [SerializeField] public string longDesc;
        [SerializeField] public List<ItemAbility> itemAbilities;
        [SerializeField] public List<string> tagResists;
        public string itemID
        {
            get{
                var itemType = this.GetType();

                string itemTypeString;

                if(this.GetType() == typeof(Armor.Helmet))
                {
                    itemTypeString = "Helmet";
                }
                else if(this.GetType() == typeof(Armor.Robe))
                {
                    itemTypeString = "Robe";
                }
                else if(this.GetType() == typeof(Armor.Glove))
                {
                    itemTypeString = "Glove";
                }
                else if(this.GetType() == typeof(Armor.Greave))
                {
                    itemTypeString = "Greave";
                }
                else if(this.GetType() == typeof(Weapon))
                {
                    itemTypeString = "Weapon";
                }
                else if(this.GetType() == typeof(Material))
                {
                    itemTypeString = "Material";
                }
                else 
                {
                    itemTypeString = "General";
                }

                return string.Format("{0}-{1}-{2}-{3}", itemTypeString, zone, level, name);
            }
        }

        public string type
        {
            get{
                var itemType = this.GetType();

                string itemTypeString;

                if(this.GetType() == typeof(Armor.Helmet))
                {
                    itemTypeString = "Helmet";
                }
                else if(this.GetType() == typeof(Armor.Robe))
                {
                    itemTypeString = "Armor";
                }
                else if(this.GetType() == typeof(Armor.Glove))
                {
                    itemTypeString = "Glove";
                }
                else if(this.GetType() == typeof(Armor.Greave))
                {
                    itemTypeString = "Shoes";
                }
                else if(this.GetType() == typeof(Weapon))
                {
                    itemTypeString = "Weapon";
                }
                else if(this.GetType() == typeof(Material))
                {
                    itemTypeString = "Material";
                }
                else 
                {
                    itemTypeString = "Accessory";
                }

                return itemTypeString;
            }            
        }

        void OnEnable()
        {

        }
        

        public int GetMaxStack(){return maxStack;}
        public int GetMaxWornCount(){return maxWornCount;}

        public Sprite GetSprite(){return itemSprite; }
        private void Reset()
        {
            maxStack = 10;
            legacyID = itemID;
        }

        public virtual string GetAdditionalInfo() { return ""; }

        public virtual List<ItemAbility> GetItemAbility() { return itemAbilities; }

        public List<string> GetTagResists()
        {
            return tagResists;
        }

        public bool hasResist()
        {
            return tagResists.Count > 0;
        }

        public bool findAbility(string ability) {
            for (int i = 0; i < itemAbilities.Count; i++)
            {
                if(itemAbilities[i].GetAbility().GetAbility().name.Contains(ability))
                {
                    return true;
                }
            } 
            return false;
        }

        public bool findAbility(List<String> ability) {
            for (int i = 0; i < itemAbilities.Count; i++)
            {
                for (int j = 0; j < ability.Count; j++)
                {
                    if(itemAbilities[i].GetAbility().GetAbility().name.Contains(ability[j]))
                    {
                        return true;
                    }
                }
            } 
            return false;
        }

        public double getAbilityPower(string abilityName)
        {
            for (int i = 0; i < itemAbilities.Count; i++)
            {
                if (itemAbilities[i].GetAbility().GetAbility().name.Contains(abilityName))
                {
                    return itemAbilities[i].GetAbility().GetRawAmount();
                }
            }
            return 0;
        }

        public bool hasPassiveAbility()
        {
            for (int i = 0; i < itemAbilities.Count; i++)
            {
                if(itemAbilities[i].IsPassive())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
