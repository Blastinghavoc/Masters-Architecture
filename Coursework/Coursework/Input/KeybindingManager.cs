using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Coursework.Input
{
    public delegate void PointerAction(Point pointerLocation);

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

        private struct MouseBinding
        {
            MouseButton button;
            InputState state;

            public MouseBinding(MouseButton button, InputState state)
            {
                this.button = button;
                this.state = state;
            }
        }

        private InputManager inputManager = new InputManager();

        /*Rather than passing any event arguments, clients simpy declare an Action for each and 
         * only the key events they care about. E.g, void OnLeftDown()
         */
        private Dictionary<KeyBinding, Action> keyBindings = new Dictionary<KeyBinding, Action>();

        /*
         Clients of mouse events declare a pointer action for any events they
         care about. This passes the position of the pointer/cursor.
        */
        private Dictionary<MouseBinding, PointerAction> mouseBindings = new Dictionary<MouseBinding, PointerAction>();

        public KeybindingManager()
        {
            inputManager.OnKeyDown += OnKeyDown;
            inputManager.OnKeyHeld += OnKeyHeld;
            inputManager.OnKeyUp += OnKeyUp;

            inputManager.OnMouseButtonDown += OnMouseButtonDown;
            inputManager.OnMouseButtonHeld += OnMouseButtonHeld;
            inputManager.OnMouseButtonUp += OnMouseButtonUp;
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

        public void BindPointerEvent(MouseButton button,InputState inputState,PointerAction action)
        {
            mouseBindings.Add(new MouseBinding(button, inputState), action);
            inputManager.ListenFor(button);
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

        void OnMouseButtonDown(object sender, MouseEventArgs e)
        {
            PointerAction action = null;

            Point pointerPosition = e.currentState.Position;

            if (mouseBindings.TryGetValue(new MouseBinding(e.button, InputState.down), out action))
            {
                action?.Invoke(pointerPosition);
            }
        }

        void OnMouseButtonUp(object sender, MouseEventArgs e)
        {
            PointerAction action = null;

            Point pointerPosition = e.currentState.Position;

            if (mouseBindings.TryGetValue(new MouseBinding(e.button, InputState.up), out action))
            {
                action?.Invoke(pointerPosition);
            }
        }

        void OnMouseButtonHeld(object sender, MouseEventArgs e)
        {
            PointerAction action = null;

            Point pointerPosition = e.currentState.Position;

            if (mouseBindings.TryGetValue(new MouseBinding(e.button, InputState.held), out action))
            {
                action?.Invoke(pointerPosition);
            }
        }


    }
    
}
