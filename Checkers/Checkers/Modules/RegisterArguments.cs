// <copyright file="RegisterArguments.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Discord.Commands;
    using Discord.WebSocket;

    /// <summary>
    /// Argument Params for the register command.
    /// </summary>
    [NamedArgumentType]
    public class RegisterArguments
    {
        public string Name { get; set; }

        public int Id { get; set; }
    }
}
