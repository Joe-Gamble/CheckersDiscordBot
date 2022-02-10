using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public class Role
    {
        public Role()
        {
            type = RoleType.Undefined;
            rating = new Rating(); ;
        }

        public Role(RoleType newType, Rating newRating)
        {
            type = newType;
            rating = newRating;
        }

        public void AddPoints(int points)
        {
            rating.Gain(points);
        }

        public void LosePoints(int points)
        {
            rating.Lose(points);
        }

        public RoleType type { get; set; }

        public bool isSelected = false;
        public bool isPlayersBest = false;

        public string icon = ":joy:";
        public Rating rating = new Rating();
    }
}
