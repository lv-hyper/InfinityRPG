using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame.Data.Item.Group
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Superior Order")]
    public class SuperiorOrder : ItemGroup
    {
        [SerializeField] int maxItemCount;

        public override void EnrollToCollection()
        {
            SuperiorOrderCollection.GetInstance().AddSuperiorOrder(this);
        }

        public bool ValidateItems(List<Item> items)
        {
            return items.FindAll(
                item => { 
                    return this.items.Contains(item); 
                }
            ).Count() <= maxItemCount;
        }

        public List<Item> FilterItems(List<Item> items)
        {
            List<Item> nonMatching = items.FindAll(
                item =>
                {
                    return !this.items.Contains(item);
                }
            ).ToList();

            List<Item> matching = items.FindAll(
                item =>
                {
                    return this.items.Contains(item);
                }
            ).ToList();

            matching.Sort((a, b) => { 
                return this.items.IndexOf(a).CompareTo(this.items.IndexOf(b));
            });

            if(matching.Count > maxItemCount)
                matching = matching.GetRange(0, maxItemCount);

            items = nonMatching;
            items.AddRange(matching);

            return items;            
        }
    }

    public class SuperiorOrderCollection
    {
        static SuperiorOrderCollection instance = null;

        List<SuperiorOrder> superiorOrders;
        Dictionary<string, List<SuperiorOrder>> itemIDIndex;
        
        SuperiorOrderCollection()
        {
            superiorOrders = new List<SuperiorOrder>();
            itemIDIndex = new Dictionary<string, List<SuperiorOrder>>();
        }

        public static SuperiorOrderCollection GetInstance()
        {
            if (instance == null)
                instance = new SuperiorOrderCollection();

            return instance;
        }

        public void AddSuperiorOrder(SuperiorOrder superiorOrder)
        {
            superiorOrders.Add(superiorOrder);

            foreach(var item in superiorOrder.GetItems())
            {
                if(!itemIDIndex.ContainsKey(item.itemID))
                {
                    itemIDIndex[item.itemID] = new List<SuperiorOrder>();
                }
                itemIDIndex[item.itemID].Add(superiorOrder);
            }
        }

        public List<SuperiorOrder> GetSuperiorOrderByItemID(string itemID)
        {
            if (itemIDIndex.ContainsKey(itemID))
                return itemIDIndex[itemID];

            else
                return null;
        }

        public List<Item> FilterItems(List<Item> items)
        {
            foreach(var superiorOrder in superiorOrders)
            {
                items = superiorOrder.FilterItems(items);
            }
            return items;
        }

        public List<SuperiorOrder> GetSuperiorOrderByItemIDList(List<string> itemIDList)
        {
            List<SuperiorOrder> itemSets = new List<SuperiorOrder>();
            foreach (var itemID in itemIDList)
            {
                var superiorOrder = GetSuperiorOrderByItemID(itemID);
                if (superiorOrder != null)
                    itemSets.AddRange(superiorOrder);
            }

            return itemSets;
        }
    }
}
