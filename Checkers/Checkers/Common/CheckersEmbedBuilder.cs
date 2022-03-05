// <copyright file="CheckersEmbedBuilder.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Services;
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
            this.WithColor(CheckersConstants.CheckerBeige);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> A Welcome Embed with rules etc. </returns>
        public Embed GetWelcomeEmbed()
        {
            return this.Build();
        }
    }
}
