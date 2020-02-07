using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ChaseCameraSample
{
    class InputListener
    {
        // Current and previous keyboard states
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }

        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }



        // List of keys to check for
        public HashSet<Keys> KeyList;

        private HashSet<MouseButton> MouseButtonList;

        //Keyboard event handlers
        //key is down
        public event EventHandler<KeyboardEventArgs> OnKeyDown = delegate { };
        //key was up and is now down
        public event EventHandler<KeyboardEventArgs> OnKeyPressed = delegate { };
        //key was down and is now up
        public event EventHandler<KeyboardEventArgs> OnKeyUp = delegate { };

        //Mouse event handlers
        public event EventHandler<MouseEventArgs> OnMouseButtonDown = delegate { };

        public InputListener()
        {
            CurrentKeyboardState = Keyboard.GetState();
            PrevKeyboardState = CurrentKeyboardState;
            CurrentMouseState = Mouse.GetState();
            PrevMouseState = CurrentMouseState;
            KeyList = new HashSet<Keys>();
            MouseButtonList = new HashSet<MouseButton>();
        }

        public void AddKey(Keys key)
        {
            KeyList.Add(key);
        }

        public void AddButton(MouseButton button)
        {
            MouseButtonList.Add(button);
        }

        public void Update()
        {
            PrevKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PrevMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            FireKeyboardEvents();
            FireMouseEvents();
        }

        private void FireMouseEvents() {
            foreach (var button in MouseButtonList)
            {
                if (button.Equals(MouseButton.LEFT))
                {
                    if (CurrentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (OnMouseButtonDown != null)
                        {
                            OnMouseButtonDown(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));

                        }
                    }
                }
            }
        }

        private void FireKeyboardEvents()
        {
            foreach (var key in KeyList)
            {
                // Is the key currently down?
                if (CurrentKeyboardState.IsKeyDown(key))
                {
                    // Fire the OnKeyDown event
                    if (OnKeyDown != null)
                    {
                        OnKeyDown(this, new KeyboardEventArgs(key, CurrentKeyboardState,
                            PrevKeyboardState));
                    }
                }

                // Has the key been released? (Was down and is now up)
                if (PrevKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key))
                {
                    // Fire the OnKeyUp event
                    if (OnKeyUp != null)
                    {
                        OnKeyUp(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));

                    }
                }

                // Has the key been pressed? (Was up and is now down)
                if (PrevKeyboardState.IsKeyUp(key) && CurrentKeyboardState.IsKeyDown(key))
                {
                    
                    if (OnKeyPressed != null)
                    {
                        OnKeyPressed(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));

                    }
                }
            }
        }
    }
}
