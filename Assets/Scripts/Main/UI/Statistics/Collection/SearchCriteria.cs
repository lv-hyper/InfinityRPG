using System.Collections.Generic;
using System.Linq;
using InGame.Data.Mob;

namespace Main.UI.Statistics.Collection
{
    
    public class SearchCriteria
    {
        private string difficulty;
        private int? soulLevel;

        public SearchCriteria()
        {
            difficulty = null;
        }

        public SearchCriteria Difficulty(string _difficulty)
        {
            this.difficulty = _difficulty;
            return this;
        }

        public SearchCriteria SoulLevel(int _soulLevel)
        {
            this.soulLevel = _soulLevel;
            return this;
        }
            
        public bool IsValid(AbstractMob mob)
        {
            if (!string.IsNullOrEmpty(difficulty) && difficulty != mob.GetDifficulty())
                return false;
                
            if (soulLevel.HasValue && soulLevel != 0 && soulLevel != mob.GetSoulLevel())
                return false;
                
            if (mob.IsHidden()) 
                return false;
                
            if (mob.GetDropTable().Count == 0 && !mob.IsBoss()) 
                return false;

            return true;
        }

        public IEnumerable<AbstractMob> Filter(IEnumerable<AbstractMob> mobs)
        {;
            return mobs.Where(mob => IsValid(mob));
        }
    }
}