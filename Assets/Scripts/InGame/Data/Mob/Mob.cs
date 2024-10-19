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
    }
}

