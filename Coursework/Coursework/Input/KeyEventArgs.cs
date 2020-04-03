using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Coursework.Input
{    
    //Very simple eventargs class for key events
    class KeyEventArgs: EventArgs
    {
        public readonly Keys Key;

        public KeyEventArgs(Keys key)
        {
            Key = key;
        }
    }
    
}
