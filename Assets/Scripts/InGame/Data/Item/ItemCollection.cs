using UnityEngine;
using System.Collections.Generic;

namespace InGame.Data.Item
{
    public class ItemCollection
    {
        public class ItemStatus
        {
            Item item;
            bool stared;
            int stack;
            int maxStack
            {
                get
                {
                    return item.GetMaxStack();
                }
            }

            public string name { get { return item.name; } }

            public ItemStatus(Item item)
            {
                this.item = item;
                this.stack = 0;
            }

            public Item getItem() { return item; }
            public int getCount() { return stack; }
            public int getMaxCount() { return maxStack; }

            public void addCount(int count = 1) { stack += count; Debug.Log(stack); }
            public void removeCount(int count = 1) { stack -= count; Debug.Log(stack); }
            public void setCount(int count) { stack = count; }
            public bool isStared() {return stared; }
            public void setStared() {stared = true; }
            public void setUnStared() {stared = false; }
        }
        private static ItemCollection instance;
        public Dictionary<string, ItemStatus> allCollection;

        Dictionary<string, ItemStatus> weaponCollection;
        Dictionary<string, ItemStatus> helmetCollection;
        Dictionary<string, ItemStatus> robeCollection;
        Dictionary<string, ItemStatus> gloveCollection;
        Dictionary<string, ItemStatus> greaveCollection;

        Dictionary<string, ItemStatus> accessoryCollection;
        Dictionary<string, ItemStatus> materialCollection;

        public static ItemCollection GetInstance()
        {
            if (instance == null)
            {
                instance = new ItemCollection();
            }

            return instance;
        }

        ItemCollection()
        {
            allCollection = new Dictionary<string, ItemStatus>();

            weaponCollection = new Dictionary<string, ItemStatus>();

            helmetCollection = new Dictionary<string, ItemStatus>();
            robeCollection = new Dictionary<string, ItemStatus>();
            gloveCollection = new Dictionary<string, ItemStatus>();
            greaveCollection = new Dictionary<string, ItemStatus>();

            accessoryCollection = new Dictionary<string, ItemStatus>();
            materialCollection = new Dictionary<string, ItemStatus>();
        }

        public void AddItem(Item item)
        {
            ItemStatus itemStatus = new ItemStatus(item);

            if (allCollection.ContainsKey(item.itemID))
            {
                return;
            }
            else
            {
                allCollection.Add(item.itemID, itemStatus);
            }

            if (item.GetType() == typeof(Weapon))
            {
                weaponCollection.Add(item.itemID, itemStatus);
            }
            else if (item.GetType() == typeof(Armor.Helmet))
            {
                helmetCollection.Add(item.itemID, itemStatus);
            }
            else if (item.GetType() == typeof(Armor.Robe))
            {
                robeCollection.Add(item.itemID, itemStatus);
            }
            else if (item.GetType() == typeof(Armor.Glove))
            {
                gloveCollection.Add(item.itemID, itemStatus);
            }
            else if (item.GetType() == typeof(Armor.Greave))
            {
                greaveCollection.Add(item.itemID, itemStatus);
            }
            else if (item.GetType() == typeof(Material))
            {
                materialCollection.Add(item.itemID, itemStatus);
            }
            else
            {
                accessoryCollection.Add(item.itemID, itemStatus);
            }
        }

        public void RemoveItemCount(string itemID, int count = 1)
        {
            ItemStatus itemStatus = allCollection[itemID];

            if (itemStatus.getCount() - count < 0)
            {
                count = itemStatus.getCount();
                if (count == 0) return;
            }


            itemStatus.removeCount(count);
            UpdateCollection(itemID, itemStatus);
            Entity.Character.GetInstance().CorrectEquipmentSet();
        }

        public void AddItemCount(string itemID, int count = 1)
        {
            ItemStatus itemStatus = allCollection[itemID];

            if (itemStatus.getMaxCount() < itemStatus.getCount() + count)
            {
                count = itemStatus.getMaxCount() - itemStatus.getCount();
                if (count == 0) return;
            }

            itemStatus.addCount(count);
            UpdateCollection(itemID, itemStatus);
        }

        public void SetItemCount(string itemID, int count)
        {
            ItemStatus itemStatus = null;
            if (allCollection.ContainsKey(itemID))
            {
                itemStatus = allCollection[itemID];
            }

            if (itemStatus == null)
            {
                Debug.LogWarning("Error : No such item : " + itemID);
                return;
            }

            if (itemStatus.getMaxCount() < count)
            {
                count = itemStatus.getMaxCount();
            }

            itemStatus.setCount(count);
            UpdateCollection(itemID, itemStatus);
        }

        public void UpdateCollection(string itemID, ItemStatus itemStatus)
        {
            allCollection[itemID] = itemStatus;


            if (itemStatus.getItem().GetType() == typeof(Weapon))
            {
                weaponCollection[itemID] = itemStatus;
            }
            else if (itemStatus.getItem().GetType() == typeof(Armor.Helmet))
            {
                helmetCollection[itemID] = itemStatus;
            }
            else if (itemStatus.getItem().GetType() == typeof(Armor.Robe))
            {
                robeCollection[itemID] = itemStatus;
            }
            else if (itemStatus.getItem().GetType() == typeof(Armor.Glove))
            {
                gloveCollection[itemID] = itemStatus;
            }
            else if (itemStatus.getItem().GetType() == typeof(Armor.Greave))
            {
                greaveCollection[itemID] = itemStatus;
            }
            else if (itemStatus.getItem().GetType() == typeof(Material))
            {
                materialCollection[itemID] = itemStatus;
            }
            else
            {
                accessoryCollection[itemID] = itemStatus;
            }
        }


        public Dictionary<string, ItemStatus> GetCollection(string choice)
        {
            Dictionary<string, ItemStatus> itemCollection;
            switch (choice)
            {
                case "weapon":
                    itemCollection = ItemCollection.GetInstance().weaponCollection;
                    break;
                case "helmet":
                    itemCollection = ItemCollection.GetInstance().helmetCollection;
                    break;
                case "robe":
                    itemCollection = ItemCollection.GetInstance().robeCollection;
                    break;
                case "glove":
                    itemCollection = ItemCollection.GetInstance().gloveCollection;
                    break;
                case "greave":
                    itemCollection = ItemCollection.GetInstance().greaveCollection;
                    break;
                case "material":
                    itemCollection = ItemCollection.GetInstance().materialCollection;
                    break;
                default:
                    if (choice.StartsWith("accessory"))
                    {
                        itemCollection = ItemCollection.GetInstance().accessoryCollection;
                    }
                    else
                    {
                        itemCollection = ItemCollection.GetInstance().allCollection;
                    }
                    break;
            }
            return itemCollection;
        }

        public void ClearCollection()
        {
            ResetDictionary(allCollection);
            ResetDictionary(weaponCollection);
            ResetDictionary(helmetCollection);
            ResetDictionary(robeCollection);
            ResetDictionary(gloveCollection);
            ResetDictionary(greaveCollection);
            ResetDictionary(accessoryCollection);
            ResetDictionary(materialCollection);
        }

        void ResetDictionary(Dictionary<string, ItemStatus> dictionary)
        {
            foreach (var element in dictionary)
            {
                element.Value.setCount(0);
            }
        }

        public List<ItemAbility> GetPassiveAbilities()
        {
            List<ItemAbility> passiveAbilities = new List<ItemAbility>();
            foreach(var itemID in allCollection.Keys)
            {
                ItemStatus itemStatus = allCollection[itemID];

                Item item = itemStatus.getItem();

                if (item.GetType() == typeof(Material))
                    continue;

                int quantity = 1;

                if(item.GetType() == typeof(Item))
                {
                    quantity = itemStatus.getCount();

                    if (quantity > itemStatus.getItem().maxWornCount)
                        quantity = itemStatus.getItem().maxWornCount;
                }


                for (int i = 0; i < quantity; ++i)
                {
                    bool nonPassiveItem = true;
                    foreach (var ability in item.itemAbilities)
                    {
                        if (ability.IsPassive())
                        {
                            passiveAbilities.Add(ability);
                            nonPassiveItem = false;
                        }
                    }
                    if (nonPassiveItem) break;
                }
            }

            return passiveAbilities;
        }

        public Item FindSingleItem(string itemID)
        {
            foreach(var ID in allCollection.Keys)
            {
                ItemStatus itemStatus = allCollection[itemID];
                
                if (ID == itemID)
                {
                    return itemStatus.getItem();
                }
            }

            return null;
        }
    }
}