using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Item
{
    [Serializable]
    public class CraftIngredient
    {
        [SerializeField] Data.Item.Item item;
        [SerializeField] int quantity;

        public Data.Item.Item GetItem() { return item; }
        public int GetQuantity() { return quantity; }
    }


    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemCraft")]
    public class Craft : ScriptableObject
    {
        [SerializeField] List<CraftIngredient> ingredients;
        [SerializeField] Data.Item.Item result;

        public List<CraftIngredient> GetCraftIngredients() { return ingredients; }
        public Data.Item.Item GetResult() { return result; }

        public void SetResult(Data.Item.Item item) { result = item; }

    }
}
