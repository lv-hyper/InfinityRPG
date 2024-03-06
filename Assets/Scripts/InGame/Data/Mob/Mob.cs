using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InGame.Data.Item;
using System.Numerics;

namespace InGame.Data.Mob
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mob/Mob")]
    public class Mob : AbstractMob
    {
        [SerializeField] protected long minLv, maxLv;
        public override AbstractMob newMobInstance(BigInteger level)
        {
            Mob newMobInstance = CreateInstance<Mob>();
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
            return newMobInstance;
        }
    }
}

