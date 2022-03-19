// <copyright file="CheckersConstants.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

using Checkers.Components;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public static class CheckersConstants
    {
        public const ulong GeneralText = 942528248099790902;
        public static ulong QueueVoice = 953440503968448512;
        public static ulong LobbyVoice = 953431550572245052;
        public static ulong RegisterRole = 942533679027200051;

        public static int MapVoteDuration = 60;
        public static int StandardWin = 25;
        public static int MaxRank = 5000;
        public static int QueueSize = 2;

        public static Color CheckerGreen = new Color(87, 203, 147);
        public static Color CheckerBeige = new Color(249, 214, 150);
        public static Color CheckerRed = Color.Red;

        public static Dictionary<string, MapType> Maps = new Dictionary<string, MapType>() {
        { "Temple of Anubis", MapType.Assault },
        { "Volskya Industries", MapType.Assault },
        { "Hanamura", MapType.Assault },
        { "Horizon Lunar Colony",  MapType.Assault },
        { "Paris", MapType.Assault },
        { "Dorado", MapType.Escort },
        { "Havana", MapType.Escort },
        { "Junkertown", MapType.Escort },
        { "Rialto" , MapType.Escort },
        { "Route 66", MapType.Escort },
        { "Watchpoint: Gibraltar",  MapType.Escort },
        { "Busan", MapType.Control },
        { "Ilios", MapType.Control },
        { "Lijiang Tower", MapType.Control },
        { "Nepal",  MapType.Control },
        { "Oasis", MapType.Control },
        { "Blizzard World", MapType.Hybrid },
        { "Eichenwalde", MapType.Hybrid },
        { "Hollywood", MapType.Hybrid },
        { "Kings Row", MapType.Hybrid },
        { "Numbani", MapType.Hybrid },
        };
    }
}
