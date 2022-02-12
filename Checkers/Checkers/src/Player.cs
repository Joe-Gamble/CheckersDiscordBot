using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public class Player
    {
        public PlayerData data;

        public Player()
        {
            data = new PlayerData();
            data.Roles = new Dictionary<RoleType, Role>();

            Role Tank = new (RoleType.Tank, new Rating());
            Role Dps = new (RoleType.Dps, new Rating());
            Role Support = new (RoleType.Support, new Rating());

            data.Roles.Add(RoleType.Tank, Tank);
            data.Roles.Add(RoleType.Dps, Dps);
            data.Roles.Add(RoleType.Support, Support);
        }

        public Role GetBestRole()
        {
            var bestRole = from r in data.Roles.Values 
                           orderby r.rating ascending
                           select r;
            return (Role)bestRole;
        }
    }
}
