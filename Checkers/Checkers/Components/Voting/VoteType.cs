// <copyright file="VoteType.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Components.Voting
{
    public enum VoteType
    {
        EndMatch,
        Forfeit,
        Disconnect,
        MapPick,
        AcceptMatch,
    }
}
