using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InGame.Data.Item;
using System.Numerics;
using System.Linq;
using System;

namespace InGame.Data.Mob
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mob/Boss")]
    public class Boss : AbstractMob
    {
        [Serializable]
        public struct ElementalSoulData
        {
            [SerializeField] public EnumElemental elementalType;
            [SerializeField] public int count;
        }

        [SerializeField] public int soulLv = 1, progressionLevel = 0;
        [SerializeField] List<ElementalSoulData> elementalSoulDrops;
        public override float getDefenceRatio(float characterDefIgnore, EnumEntityClass attackType)
        {
            float defRatio = maxDefenceRatio;

            defRatio *= 1.3f;

            defRatio = defRatio * (100.0f - characterDefIgnore) / 100.0f;

            if (defRatio < 1.0f) defRatio = 1.0f;

            return defRatio / 2.0f;
        }
        public override BigInteger GetHP()
        {

            return base.GetHP() * 5 / 3;
        }
        public override BigInteger GetATK()
        {
            return base.GetATK() * 105 / 100;
        }
        public override BigInteger GetEXP()
        {
            return base.GetEXP() * 3 / 2;
        }

        public override BigInteger GetGold()
        {
            return base.GetGold() * 2;
        }

        public override AbstractMob newMobInstance(BigInteger level)
        {
            Boss newMobInstance = CreateInstance<Boss>();

            newMobInstance.InitNewMob(
                name,
                mobSprite, imageOffset,
                description,
                mobClass,
                elementalDamage, elementalDefence,
                level, hpRate, atkRate, expRate, goldRate, defReqRate, customHP, customATK,
                integratedElementalDefence,
                dropTable,
                tagList,
                skills
            );
            newMobInstance.SetSoulLevel(soulLv);
            newMobInstance.SetProgressionLevel(progressionLevel);

            return newMobInstance;
        }
        public override bool IsBoss()
        {
            return true;
        }

        public void SetSoulLevel(int lv)
        {
            soulLv = lv;
        }

        public void SetProgressionLevel(int lv)
        {
            progressionLevel = lv;
        }

        public override int GetSoulLevel()
        {
            return soulLv;
        }

        public int GetProgressionLevel()
        {
            return progressionLevel;
        }

        public override bool Extracted()
        {
            return false;
        }

        public List<ElementalSoulData> GetElementalSoulDrops() { return elementalSoulDrops; }

        public int GetElementalSoulCount(EnumElemental _elementalType)
        {
            return elementalSoulDrops.Where(x => x.elementalType == _elementalType).Sum(x => x.count);
        }

    }
}

