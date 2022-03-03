// <copyright file="CheckersComponentBuilder.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Components.Voting;
    using Discord;

    /// <summary>
    /// Builder class for cpomponent messages for Checkers.
    /// </summary>
    public class CheckersComponentBuilder : ComponentBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckersComponentBuilder"/> class.
        /// </summary>
        public CheckersComponentBuilder(VoteType type, bool disabled)
        {
            switch (type)
            {
                case VoteType.EndMatch:
                    {
                        this.WithButton("Vote Yes", "match_voteyes", ButtonStyle.Success, null, null, disabled, 0)
                            .WithButton("Vote No", "match_voteno", ButtonStyle.Danger, null, null, disabled, 0);
                        break;
                    }
            }
        }
    }
}
