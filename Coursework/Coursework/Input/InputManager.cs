using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Input
{
    /// <summary>
    /// Based on Lab 3, detects user input and fires relevant events.
    /// </summary>
    class InputManager
    {
        // Current and previous keyboard states
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }

        // List of keys to check for
        private HashSet<Keys> KeySet = new HashSet<Keys>();
        //List of mouse buttons to check for
        private HashSet<MouseButton> ButtonSet = new HashSet<MouseButton>();

        //Keyboard event handlers
        //key was up and is now down
        public event EventHandler<KeyEventArgs> OnKeyDown = delegate { };
        //key is held down
        public event EventHandler<KeyEventArgs> OnKeyHeld = delegate { };
        //key was down and is now up
        public event EventHandler<KeyEventArgs> OnKeyUp = delegate { };

        //Mouse event handlers
        public event EventHandler<MouseEventArgs> OnMouseButtonDown = delegate { };
        public event EventHandler<MouseEventArgs> OnMouseButtonHeld = delegate { };
        public event EventHandler<MouseEventArgs> OnMouseButtonUp = delegate { };

        public InputManager() {
            PrevKeyboardState = Keyboard.GetState();
            CurrentKeyboardState = PrevKeyboardState;
            PrevMouseState = Mouse.GetState();
            CurrentMouseState = PrevMouseState;
        }

        public void Update()
        {
            //Update keyboard states
            PrevKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            //Update mouse states
            PrevMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

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

            foreach (var button in ButtonSet)
            {
                bool isDown = false;
                bool wasDown = false;

                switch (button)
                {
                    case MouseButton.left:
                        {
                            isDown = CurrentMouseState.LeftButton == ButtonState.Pressed;
                            wasDown = PrevMouseState.LeftButton == ButtonState.Pressed;
                        }
                        break;
                    case MouseButton.right:
                        {
                            isDown = CurrentMouseState.RightButton == ButtonState.Pressed;
                            wasDown = PrevMouseState.RightButton == ButtonState.Pressed;
                        }
                        break;
                    default:
                        break;
                }

                if (isDown)//Button is down
                {
                    OnMouseButtonHeld?.Invoke(this, new MouseEventArgs(button,CurrentMouseState));

                    if (!wasDown)//Button has just been pressed
                    {
                        OnMouseButtonDown?.Invoke(this, new MouseEventArgs(button,CurrentMouseState));
                    }
                }
                else
                {//Button is not down
                    if (wasDown)//Button has just been released
                    {
                        OnMouseButtonUp?.Invoke(this, new MouseEventArgs(button,CurrentMouseState));
                    }
                }

            }
        }

        //Start listening for the given key
        public void ListenFor(Keys key)
        {
            KeySet.Add(key);
        }

        public void ListenFor(MouseButton button) {
            ButtonSet.Add(button);
        }

    }


}
