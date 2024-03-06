using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data.Item.Group
{
    [Serializable]
    public abstract class ItemGroup : ScriptableObject
    {
        [SerializeField] protected List<Item> items;

        public abstract void EnrollToCollection();

        public List<Item> GetItems() { return items; }
    }

}
