using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public class Rating
    {
        public int currentRating { get; set; }
        public SkillTier currentTier { get; set; }

        private int gamesOutOfDivision = 0;

        public Rating()
        {
            currentRating = 0;
            currentTier = SkillTier.Undefined;
        }
        public Rating(int rating, SkillTier tier)
        {
            currentRating = rating;
            currentTier = tier;
        }

        public void Gain(int total)
        {
            currentRating += total;

            SkillTier tier = GetTierAt(currentRating);

            if (currentTier != tier)
            {
                //promoted
                currentTier = GetTierAt(currentRating);
            }
        }

        public void Lose(int total)
        {
            currentRating -= total;

            if(GetTierAt(currentRating) != currentTier)
            {
                gamesOutOfDivision++;

                if (gamesOutOfDivision >= 5)
                {
                    currentTier = GetTierAt(currentRating);
                    gamesOutOfDivision = 0;
                }
            }
        }

        //this might need to be public or moved
        private SkillTier GetTierAt(int total)
        {
            if (total < 0) return SkillTier.Undefined;
            else if (total <= 1500) return SkillTier.Bronze;
            else if (total <= 2000) return SkillTier.Silver;
            else if (total <= 2500) return SkillTier.Gold;
            else if (total <= 3000) return SkillTier.Platnium;
            else if (total <= 3500) return SkillTier.Diamond;
            else if (total <= 4000) return SkillTier.Masters;
            else if (total <= 5000) return SkillTier.Grandmasters;

            return SkillTier.Undefined;
        }
    }
}
