namespace Checkers.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Channel Types significant to the Checkers server.
    /// </summary>
    public enum ChannelTypes
    {
        /// <summary>
        /// The Registry.
        /// </summary>
        Registry = 0,

        /// <summary>
        /// Lobby Voice, should be restricted to one.
        /// </summary>
        LobbyVoice,

        /// <summary>
        /// Team voice channels, should be used to define permissions on creation etc.
        /// </summary>
        TeamVoice,
    }
}
