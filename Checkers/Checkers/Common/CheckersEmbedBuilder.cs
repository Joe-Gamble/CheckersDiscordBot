namespace Checkers.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Discord;

    /// <summary>
    /// Builder class for themed embeded messages used by Checkers.
    /// </summary>
    public class CheckersEmbedBuilder : EmbedBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckersEmbedBuilder"/> class.
        /// </summary>
        public CheckersEmbedBuilder()
        {
            this.WithColor(new Color(87, 203, 147));
        }
    }
}
