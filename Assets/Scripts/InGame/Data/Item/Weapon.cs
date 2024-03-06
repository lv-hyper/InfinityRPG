using UnityEngine;
using System;
using System.Collections.Generic;

namespace InGame.Data.Item
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item/Weapon")]
    public class Weapon : Item
    {
        [Serializable]
        public enum WeaponType{Melee, Staff, Ranged, Any=101}

        [SerializeField]
        public WeaponType weaponType;

        [SerializeField]
        EnumElemental elemental;

        public WeaponType GetWeaponType(){return weaponType;}

        public EnumElemental GetElemental() { return elemental;}

        private void Reset() {
            maxStack = 10;
        }

        public override string GetAdditionalInfo() {
            switch (weaponType)
            {
                case WeaponType.Melee:
                    return "This weapon requires STR to damage";
                    case WeaponType.Staff:
                    return "This weapon requires INT to damage";
                case WeaponType.Ranged:
                    return "This weapon requires DEX to damage";
                case WeaponType.Any:
                    return "This weapon requires STR/INT/DEX to damage";
                default:
                    return "";
            }
        }

        public override List<ItemAbility> GetItemAbility()
        {
            return ItemAbility.AmpiflyAbility(
                new List<string> {
                    "Attack Percent Point",
                    "Additional Attack"
                },
                base.GetItemAbility(),
                ItemCollection.GetInstance().allCollection[itemID].getCount()
            );
        }
    }
}
