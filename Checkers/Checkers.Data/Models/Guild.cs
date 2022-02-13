// <copyright file="Guild.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Checkers.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Guild
    {
        public ulong Id { get; set; }

        public string Prefix { get; set; } = "#";
    }
}
