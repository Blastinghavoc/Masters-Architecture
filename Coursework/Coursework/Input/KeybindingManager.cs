using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Coursework.Input
{

    public class KeybindingManager
    {
        private struct KeyBinding
        {
            Keys key;
            InputState state;

            public KeyBinding(Keys key, InputState state)
            {
                this.key = key;
                this.state = state;
            }
        }

        private InputManager inputManager;

        /*Rather than passing any event arguments, clients simpy declare an Action for each and 
         * only the key events they care about. E.g, void OnLeftDown()
         */
        private Dictionary<KeyBinding, Action> keyBindings;

        public KeybindingManager()
        {
            inputManager = new InputManager();
            keyBindings = new Dictionary<KeyBinding, Action>();

            inputManager.OnKeyDown += OnKeyDown;
            inputManager.OnKeyHeld += OnKeyHeld;
            inputManager.OnKeyUp += OnKeyUp;
        }

        public void Update()
        {
            inputManager.Update();
        }

        public void BindKeyEvent(Keys key, InputState inputState, Action action)
        {
            keyBindings.Add(new KeyBinding(key, inputState), action);
            inputManager.ListenFor(key);
        }

        void OnKeyDown(object sender,KeyEventArgs e) {
            Action action = null;

            if (keyBindings.TryGetValue(new KeyBinding(e.Key, InputState.down),out action)) {
                action?.Invoke();
            }           
        }

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            Action action = null;

            if (keyBindings.TryGetValue(new KeyBinding(e.Key, InputState.up), out action))
            {
                action?.Invoke();
            }
        }

        void OnKeyHeld(object sender, KeyEventArgs e)
        {
            Action action = null;

            if (keyBindings.TryGetValue(new KeyBinding(e.Key, InputState.held), out action))
            {
                action?.Invoke();
            }
        }

    }
    
}
