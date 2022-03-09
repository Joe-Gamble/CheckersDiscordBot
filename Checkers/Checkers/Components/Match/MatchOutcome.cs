﻿// <copyright file="MatchOutcome.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public enum MatchOutcome
    {
        Cancelled = -1,
        Draw,
        TeamA,
        TeamB,
        Forfeit,
        Disconnect
    }
}
