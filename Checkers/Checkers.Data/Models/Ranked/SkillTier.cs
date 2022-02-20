using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Data.Models.Ranked
{
    /// <summary>
    /// Tier Definitions for Skill Rating.
    /// </summary>
    public enum SkillTier
    {
        /// <summary>
        /// Invalid Rank / Placements
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Rating here
        /// </summary>
        Silver,

        /// <summary>
        /// Rating here
        /// </summary>
        Gold,

        /// <summary>
        /// Rating here
        /// </summary>
        Platnium,

        /// <summary>
        /// Rating here
        /// </summary>
        Sapphire,

        /// <summary>
        /// Rating here
        /// </summary>
        Masters,

        /// <summary>
        /// Rating here
        /// </summary>
        Warlord,
    }
}
