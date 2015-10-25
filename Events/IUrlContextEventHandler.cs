using Orchard.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Events
{
    public interface IUrlContextEventHandler : IEventHandler
    {
        /// <summary>
        /// ...
        /// </summary>
        void Changing(ChangingUrlContext context);
    }
}