using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    /// <summary>
    /// Interface to any class that owns content
    /// </summary>
    interface IContentOwner
    {
        //TODO Consider just using IDisposable instead?
        void ReleaseContent();
    }
}
