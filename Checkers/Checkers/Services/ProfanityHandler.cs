namespace Checkers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using ProfanityFilter;

    /// <summary>
    /// Wrapper class for handling Profanity detection.
    /// </summary>
    public class ProfanityHandler
    {
        private static ProfanityHandler instance = null;
        private readonly ProfanityFilter filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfanityHandler"/> class.
        /// </summary>
        public ProfanityHandler()
        {
            this.filter = new ProfanityFilter();
        }

        /// <summary>
        /// Gets singleton retriever for the ProfanityHandler.
        /// </summary>
        public static ProfanityHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProfanityHandler();
                }

                return instance;
            }
        }

        /// <summary>
        /// Returns the <see cref="ProfanityFilter"/> associated with this handler.
        /// </summary>
        /// <returns> The profanity filter. </returns>
        public ProfanityFilter Filter()
        {
            return this.filter;
        }
    }
}
