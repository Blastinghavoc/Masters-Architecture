using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Input
{
    /// <summary>
    /// Based on Lab 3
    /// </summary>
    class InputManager
    {
        // Current and previous keyboard states
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }

        // List of keys to check for
        private HashSet<Keys> KeySet;           

        //Keyboard event handlers
        //key was up and is now down
        public event EventHandler<KeyEventArgs> OnKeyDown = delegate { };
        //key is held down
        public event EventHandler<KeyEventArgs> OnKeyHeld = delegate { };
        //key was down and is now up
        public event EventHandler<KeyEventArgs> OnKeyUp = delegate { };

        public InputManager() {
            KeySet = new HashSet<Keys>();
            PrevKeyboardState = Keyboard.GetState();
            CurrentKeyboardState = PrevKeyboardState;
        }

        public void Update()
        {
            //Update keyboard states
            PrevKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            foreach (var key in KeySet)
            {
                var isDown = CurrentKeyboardState.IsKeyDown(key);
                var wasDown = PrevKeyboardState.IsKeyDown(key);

                if (isDown)//Key is down
                {
                    //Key is currently held
                    OnKeyHeld?.Invoke(this, new KeyEventArgs(key));

                    if (!wasDown)//Key has just been pressed
                    {
                        OnKeyDown?.Invoke(this, new KeyEventArgs(key));
                    }
                }
                else {//Key is not down
                    if (wasDown)//Key has just been released
                    {
                        OnKeyUp?.Invoke(this, new KeyEventArgs(key));
                    }
                }
            }
        }

        //Start listening for the given key
        public void ListenFor(Keys key)
        {
            KeySet.Add(key);
        }

    }

    
}
