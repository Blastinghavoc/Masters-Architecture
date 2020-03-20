using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    /// <summary>
    /// Represents an object that subscribes to events for the duration of
    /// its lifetime, and then unsubscribes from them when it is disposed.
    /// </summary>
    interface EventSubscriber:IDisposable
    {
        //Should generally be called in the constructor to subscribe to events
        void BindEvents();
        //Should be called in the dispose method to unsubscribe from any events
        void UnbindEvents();
    }
}
