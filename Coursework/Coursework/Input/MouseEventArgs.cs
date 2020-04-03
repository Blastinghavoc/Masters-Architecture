using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Input
{
    public enum MouseButton {
        left,
        right
    }

    /// <summary>
    /// Simple EventArgs class for mouse events
    /// </summary>
    class MouseEventArgs : EventArgs
    {
        public readonly MouseButton button;
        public readonly MouseState currentState;

        public MouseEventArgs(MouseButton button, MouseState currentState)
        {
            this.button = button;
            this.currentState = currentState;
        }
    }
}
