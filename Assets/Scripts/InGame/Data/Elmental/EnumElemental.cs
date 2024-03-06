using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data
{
    [Serializable]
    public class ElementalElement
    {
        public EnumElemental elemental;
        public long amount;
    }

    public class ElementalAttribute : Attribute
    {
        public ElementalAttribute(string abilityDescription)
        {
            this.abilityDescription = abilityDescription;
        }
        public string abilityDescription { get; private set; }
    }

    [Serializable]
    public enum EnumElemental
    {
        [Elemental("Non-Elemental")]
        None,
        [Elemental("Air Elemental")]
        Air,
        [Elemental("Nature Elemental")]
        Nature,
        [Elemental("Fire Elemental")]
        Fire,
        [Elemental("Water Elemental")]
        Water,
        [InspectorName(null)]
        Count
    }

    [Serializable]
    public struct Elemental
    {
        public static ElementalAttribute GetInfo(EnumElemental elemental)
        {
            ElementalAttribute attribute =

            (ElementalAttribute)Attribute.GetCustomAttribute(
                typeof(EnumElemental).GetField(
                    Enum.GetName(
                        typeof(EnumElemental),
                        elemental
                    )
                ),
                typeof(ElementalAttribute)
            );

            return attribute;
        }

        public static EnumElemental GetStrongerElemental(EnumElemental elemental)
        {
            int elementalCount = System.Enum.GetValues(typeof(EnumElemental)).Length - 1;

            if ((int)(elemental) == elementalCount - 1)
            {
                return (EnumElemental)1;
            }
            else if ((int)(elemental) == 0)
                return elemental;
            else
                return elemental+1;
        }

        public static EnumElemental GetWeakerElemental(EnumElemental elemental)
        {
            int elementalCount = System.Enum.GetValues(typeof(EnumElemental)).Length - 1;

            if ((int)(elemental) == 1)
            {
                return (EnumElemental)(elementalCount - 1);
            }
            else if ((int)(elemental) == 0)
                return elemental;
            else
                return elemental - 1;
        }

        public static float GetElementalAdvantageRate(EnumElemental thisElemental, EnumElemental otherElemental)
        {
            if (Elemental.GetStrongerElemental(thisElemental) == otherElemental)
            {
                return 0.7f;
            }
            else
            {
                return 1.0f;
            }
        }
    }
}
