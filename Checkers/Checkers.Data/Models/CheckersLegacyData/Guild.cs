// <copyright file="Guild.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Guild Class for Checkers Data.
    /// </summary>
    public class Guild
    {
        public ulong Id { get; set; }

        public string Prefix { get; set; } = "!";
    }
}
