using UnityEngine;
using System;

namespace InGame.Data.Item
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item/Material")]
    public class Material : Item
    {
        private void Reset() {
            maxStack = 100;
        }
    }
}
