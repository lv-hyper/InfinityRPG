
using UnityEngine;
using System;
using System.Collections.Generic;

namespace InGame.Data.Item
{
    [Serializable]
    public class EquipmentSet
    {
        [SerializeField] string setName;
        [SerializeField] Data.Item.Weapon weapon;

        [SerializeField] Data.Item.Armor.Helmet helmet;
        [SerializeField] Data.Item.Armor.Robe robe;
        [SerializeField] Data.Item.Armor.Glove glove;
        [SerializeField] Data.Item.Armor.Greave greave;

        [SerializeField] List<Data.Item.Item> accessories;
        [SerializeField] List<Data.Skill.ActiveSkill> skills;
        [SerializeField] List<int> skillPriorityList;

        public EquipmentSet()
        {
            setName = "New Set";

            weapon = null;

            helmet = null;
            robe = null;
            glove = null;
            greave = null;

            accessories = new List<Data.Item.Item>();
            skills = new List<Data.Skill.ActiveSkill>();
            skillPriorityList = new List<int>();

            for(int i=0;i<10;++i)
            {
                accessories.Add(null);
            }

            for(int i=0;i<6;++i)
            {
                skills.Add(null);
                skillPriorityList.Add(1);
            }
        }

        public void SetItem(string itemSlot, Item item)
        {
            switch (itemSlot)
            {
                case "weapon":
                    weapon = (Weapon)item;
                    break;
                case "helmet":
                    helmet = (Armor.Helmet)item;
                    break;
                case "robe":
                    robe = (Armor.Robe)item;
                    break;
                case "glove":
                    glove = (Armor.Glove)item;
                    break;
                case "greave":
                    greave = (Armor.Greave)item;
                    break;
                default:
                    if(itemSlot.StartsWith("accessory"))
                    {
                        int slot = int.Parse(itemSlot.Replace("accessory", "")) - 1;

                        accessories[slot] = item;
                    }
                    break;
            }


            Entity.Character.GetInstance().ApplyItemAbilities();
        }

        public void SetSkill(int skillSlot, Data.Skill.ActiveSkill skill)
        {
            skills[skillSlot] = skill;

            Entity.Character.GetInstance().CharacterStatUpdate();
        }

        public void SetSkillPriority(int skillSlot, int priority)
        {
            skillPriorityList[skillSlot] = priority;

            Entity.Character.GetInstance().CharacterStatUpdate();
        }

        public void AddSkillPriority(int skillSlot, int amount = 1)
        {
            int result = skillPriorityList[skillSlot] + amount;
            if (result > 6) result = 6;
            SetSkillPriority(skillSlot, result);
        }
        public void RemoveSkillPriority(int skillSlot, int amount = 1)
        {
            int result = skillPriorityList[skillSlot] - amount;
            if(result < 1 && skillSlot == 0) result = 1;
            if(result < 0) result = 0;
            SetSkillPriority(skillSlot, result);
        }

        public Item GetItem(string itemSlot)
        {
            switch (itemSlot)
            {
                case "weapon":
                    return weapon;
                case "helmet":
                    return helmet;
                case "robe":
                    return robe;
                case "glove":
                    return glove;
                case "greave":
                    return greave;
                default:
                    if (itemSlot.StartsWith("accessory"))
                    {
                        int slot = int.Parse(itemSlot.Replace("accessory", ""))-1;

                        return accessories[slot];
                    }
                    else return null;
            }

        }

        public Data.Skill.ActiveSkill GetSkill(int skillSlot)
        {
            if (skills.Count <= skillSlot)
                return null;
            return skills[skillSlot];
        }

        public int GetSkillPriority(int skillSlot)
        {
            if (skills.Count <= skillSlot)
                return 0;
            return skillPriorityList[skillSlot];
        }

        public string GetSetName() { return setName; }

        public void SetSetName(string setName) { this.setName = setName; }

        public List<ItemAbility> RemovePassiveEffects(List<ItemAbility> effects)
        {
            List<ItemAbility> itemAbilities = new List<ItemAbility>();

            foreach (ItemAbility ability in effects)
            {
                if (ability.IsPassive()) continue;
                
                itemAbilities.Add(ability);
            }

            return itemAbilities;
        }

        public List<ItemAbility> GetTotalAbility()
        {
            List<ItemAbility> allItemAbility = new List<ItemAbility>();
            /*
            List<Item> itemList = new List<Item>();

            var character = Entity.Character.GetInstance();

            if (weapon != null)
                itemList.Add(weapon);
            //allItemAbility.AddRange(weapon.GetItemAbility());
            if (helmet != null)
                itemList.Add(helmet);
            //allItemAbility.AddRange(helmet.GetItemAbility());
            if (robe != null)
                itemList.Add(robe);
            //allItemAbility.AddRange(robe.GetItemAbility());
            if (glove != null)
                itemList.Add(glove);
            //allItemAbility.AddRange(glove.GetItemAbility());
            if (greave != null)
                itemList.Add(greave);
            //allItemAbility.AddRange(greave.GetItemAbility());


            for (int i = 0; i < accessories.Count; ++i)
            {
                if (accessories[i] != null && !character.IsAccLocked(i))
                    itemList.Add(accessories[i]);
                //allItemAbility.AddRange(RemovePassiveEffects(accessories[i].GetItemAbility()));
            }

            itemList = Group.SuperiorOrderCollection.GetInstance().FilterItems(itemList);
            */
            List<Item> itemList = GetItemList();

            foreach(Item item in itemList)
            {
                allItemAbility.AddRange(item.GetItemAbility());
            }

            allItemAbility.AddRange(Group.ItemSetCollection.GetInstance().GetItemSetAbility(itemList));

            allItemAbility = RemovePassiveEffects(allItemAbility);

            return allItemAbility;
        }
        public List<string> GetAllTagResists()
        {
            List<string> allTagResists = new List<string>();
            List<Item> itemList = GetItemList();

            foreach(Item item in itemList)
            {
                allTagResists.AddRange(item.GetTagResists());
            }

            allTagResists.AddRange(Group.ItemSetCollection.GetInstance().GetItemSetTagResists(itemList));

            return allTagResists;
        }

        public List<Item> GetItemList()
        {
            List<Item> itemList = new List<Item>();
            var character = Entity.Character.GetInstance();

            if (weapon != null)
                itemList.Add(weapon);
            //allItemAbility.AddRange(weapon.GetItemAbility());
            if (helmet != null)
                itemList.Add(helmet);
            //allItemAbility.AddRange(helmet.GetItemAbility());
            if (robe != null)
                itemList.Add(robe);
            //allItemAbility.AddRange(robe.GetItemAbility());
            if (glove != null)
                itemList.Add(glove);
            //allItemAbility.AddRange(glove.GetItemAbility());
            if (greave != null)
                itemList.Add(greave);
            //allItemAbility.AddRange(greave.GetItemAbility());


            for (int i = 0; i < accessories.Count; ++i)
            {
                if (accessories[i] != null && !character.IsAccLocked(i))
                    itemList.Add(accessories[i]);
                //allItemAbility.AddRange(RemovePassiveEffects(accessories[i].GetItemAbility()));
            }

            return itemList;
        }

        public void InitItemCollectionWithSet()
        {
            if (weapon != null)
                ItemCollection.GetInstance().SetItemCount(weapon.itemID, 1);
            if (helmet != null)
                ItemCollection.GetInstance().SetItemCount(helmet.itemID, 1);
            if (robe != null)
                ItemCollection.GetInstance().SetItemCount(robe.itemID, 1);
            if (glove != null)
                ItemCollection.GetInstance().SetItemCount(glove.itemID, 1);
            if (greave != null)
                ItemCollection.GetInstance().SetItemCount(greave.itemID, 1);
            for (int i = 0; i < accessories.Count; ++i)
            {
                if (accessories[i] != null)
                    ItemCollection.GetInstance().SetItemCount(accessories[i].itemID, 1);
            }
        }


        public void CorrectEquipmentSet()
        {
            var collection = ItemCollection.GetInstance().allCollection;
            List<Item> order = new List<Item>();
            order.Add(weapon);
            order.Add(helmet);
            order.Add(robe);
            order.Add(glove);
            order.Add(greave);
            order.AddRange(accessories);

            for(int i=0; i<order.Count; ++i)
            {
                if (order[i] == null) continue;
                if (collection[order[i].itemID].getCount() < GetWornCount(order[i]))
                {
                    order[i] = null;
                }
            }

            weapon = (Weapon)order[0];
            helmet = (Armor.Helmet)order[1];
            robe = (Armor.Robe)order[2];
            glove = (Armor.Glove)order[3];
            greave = (Armor.Greave)order[4];
            for(int i=0; i<order.Count-5; ++i)
            {
                accessories[i] = order[i + 5];
            }
        }
        /*
        public void CorrectSkillSet()
        {
            for (int i = 1; i < 6; ++i)
            {
                if (skills[i] == null) continue;

                if (!Skill.SkillCollection.GetInstance().allSkillCollection.ContainsKey(skills[i].GetSkillID()))
                {
                    skills[i] = null;
                }

                else if(Skill.SkillCollection.GetInstance().allSkillCollection[skills[i].GetSkillID()].GetCount() == 0)
                {
                    skills[i] = null;
                }
            }
        }
        */

        public void ResetSkill()
        {
            for(int i=1; i<6; ++i)
            {
                skills[i] = null;
            }
        }

        public int GetWornCount(Item item)
        {
            int count = 0;

            if (weapon != null && item.itemID == weapon.itemID) ++count;
            if (helmet != null && item.itemID == helmet.itemID) ++count;
            if (robe != null && item.itemID == robe.itemID) ++count;
            if (glove != null && item.itemID == glove.itemID) ++count;
            if (greave != null && item.itemID == greave.itemID) ++count;

            foreach(var accessory in accessories)
            {
                if (accessory != null && item.itemID == accessory.itemID) ++count;
            }

            return count;
        }

        public bool isSkillEquiped(Data.Skill.ActiveSkill skill)
        {
            foreach (var _skill in skills)
            {
                if (_skill != null && _skill.GetSkillID() == skill.GetSkillID()) return true;
            }

            return false;
        }

    }
}