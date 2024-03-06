using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Character
{
    public class CharacterClassData
    {
        public static void AccumulateLevel(EnumEntityClass chClass, BigInteger level)
        {
            BigInteger currentLv = GetAccumulatedCharacterLevel(chClass);
            BigInteger characterLv = GetCharacterClassLevel(chClass);

            currentLv += level;

            while (currentLv > LevelUpRequirement(characterLv))
            {
                currentLv -= LevelUpRequirement(characterLv);

                PlayerPrefs.SetString(
                    string.Format("CharacterClassPoint/{0}", chClass.ToString()), 
                    (GetCharacterClassPoint(chClass)+1).ToString()
                );

                PlayerPrefs.SetString(
                    string.Format("CharacterClassLevel/{0}", chClass.ToString()), 
                    (characterLv + 1).ToString()
                );

                ++characterLv;
            }

            PlayerPrefs.SetString(string.Format("CharacterAccLevel/{0}", chClass.ToString()), currentLv.ToString());
        }
        public static BigInteger LevelUpRequirement(BigInteger lv)
        {
            if(lv <= 30)
                return (BigInteger)Mathf.Floor(Mathf.Pow(1.64f, (float)(lv - 1)) * 100000);

            else if(lv <= 50)
                return LevelUpRequirement(30) * (BigInteger)Mathf.Floor(Mathf.Pow(1.4f, (float)(lv - 30)));

            else if (lv <= 75)
                return LevelUpRequirement(50) * (BigInteger)Mathf.Floor(Mathf.Pow(1.3f, (float)(lv - 50)));

            else if (lv <= 100)
                return LevelUpRequirement(75) * (BigInteger)Mathf.Floor(Mathf.Pow(1.22f, (float)(lv - 75)));

            else
                return LevelUpRequirement(100) * (BigInteger)Mathf.Floor(Mathf.Pow(1.15f, (float)(lv - 100)));

        }

        public static void LoseCharacterClassPoint(EnumEntityClass chClass, BigInteger amount)
        {
            PlayerPrefs.SetString(
                string.Format("CharacterClassPoint/{0}", chClass.ToString()),
                (GetCharacterClassPoint(chClass) - amount).ToString()
            );
        }
        public static BigInteger GetCharacterClassPoint(EnumEntityClass chClass)
        {
            return BigInteger.Parse(PlayerPrefs.GetString(string.Format("CharacterClassPoint/{0}", chClass.ToString()), "0"));
        }

        public static void AddCharacterClassPoint(EnumEntityClass chClass, BigInteger amount)
        {
            PlayerPrefs.SetString(
                string.Format("CharacterClassPoint/{0}", chClass.ToString()),
                (GetCharacterClassPoint(chClass) + amount).ToString()
            );   
        }

        public static BigInteger GetCharacterClassLevel(EnumEntityClass chClass)
        {
            return BigInteger.Parse(PlayerPrefs.GetString(string.Format("CharacterClassLevel/{0}", chClass.ToString()), "1"));
        }

        public static BigInteger GetAccumulatedCharacterLevel(EnumEntityClass chClass)
        {
            return BigInteger.Parse(PlayerPrefs.GetString(string.Format("CharacterAccLevel/{0}", chClass.ToString()), "0")); ;
        }

        public static void ResetAllSkill()
        {
            for(EnumEntityClass ch = EnumEntityClass.Warrior; ch <= EnumEntityClass.Archer; ++ch)
            {
                PlayerPrefs.SetString(
                    string.Format("CharacterClassPoint/{0}", ch.ToString()),
                    (GetCharacterClassLevel(ch) - 1).ToString()
                );
            }
        }

        public static void ResetSpecificSkill(EnumEntityClass chClass)
        {
            PlayerPrefs.SetString(
                string.Format("CharacterClassPoint/{0}", chClass.ToString()),
                (GetCharacterClassLevel(chClass) - 1).ToString()
            );
        }
    }
}
