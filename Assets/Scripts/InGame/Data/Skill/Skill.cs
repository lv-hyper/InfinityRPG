using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{
    public abstract class Skill : ScriptableObject
    {
        [SerializeField] protected Sprite skillSprite;
        public Sprite GetSprite() { return skillSprite; }
        protected abstract string skillID { get; }
    }
}
