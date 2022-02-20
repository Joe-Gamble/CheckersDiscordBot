using Checkers.Data.Models.Ranked;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Data.Models
{
    /// <summary>
    /// Player stats.
    /// </summary>
    public class PlayerStats
    {
        /// <summary>
        /// Gets or Sets the Players SkillRating.
        /// </summary>
        public SkillRating? Rating { get; set; }

        /// <summary>
        /// Gets or Sets the Players Winrate.
        /// </summary>
        public int WinRate { get; set; }
    }
}
