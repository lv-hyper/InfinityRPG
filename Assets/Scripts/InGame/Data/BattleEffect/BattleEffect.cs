using InGame.Data.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.BattleEffect
{
    public abstract class BattleEffect : ScriptableObject
    {
        [SerializeField] Sprite effectIcon;
        [SerializeField] public int duration;
        [SerializeField] public List<SkillAbility> abilities;

        public Sprite GetIcon() { return effectIcon; }

        public abstract String GetDescription();
    }
}
