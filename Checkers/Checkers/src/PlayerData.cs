using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public struct PlayerData
    {
        public string Name { get; set; }
        public Dictionary<RoleType, Role> Roles { get; set; }
    }
}
