using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ChaseCameraSample
{
    public delegate void GameAction(eButtonState buttonState, Vector2 amount);

    public class CommandManager
    {
        private InputListener m_Input;

        private Dictionary<Keys, GameAction> m_KeyBindings = new Dictionary<Keys, GameAction>();
        private Dictionary<MouseButton, GameAction> m_MouseButtonBindings = new Dictionary<MouseButton, GameAction>();

        public CommandManager()
        {
            m_Input = new InputListener();

            // Register events with the input listener
            m_Input.OnKeyDown += this.OnKeyDown;
            m_Input.OnKeyPressed += this.OnKeyPressed;
            m_Input.OnKeyUp += this.OnKeyUp;
            m_Input.OnMouseButtonDown += this.OnMouseButtonDown;
        }

        public void Update()
        {
            // Update polling input listener, everything else is handled by events
            m_Input.Update();
        }

        public void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.DOWN, new Vector2(1.0f));
            }

        }

        public void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.UP, new Vector2(1.0f));
            }
        }

        public void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.PRESSED, new Vector2(1.0f));
            }
        }

        //
        public void OnMouseButtonDown(object sender, MouseEventArgs e)
        {
            GameAction action = m_MouseButtonBindings[e.Button];

            if (action != null)
            {
                action(eButtonState.DOWN,  new Vector2(e.CurrentState.X, e.CurrentState.Y));
            }
        }

        public void AddKeyboardBinding(Keys key, GameAction action)
        {
            // Add key to listen for when polling
            m_Input.AddKey(key);

            // Add the binding to the command map
            m_KeyBindings.Add(key, action);
        }

        public void AddMouseBinding(MouseButton button, GameAction action)
        {
            // Add key to listen for when polling
            m_Input.AddButton(button);

            // Add the binding to the command map
            m_MouseButtonBindings.Add(button, action);
        }

    }

}
