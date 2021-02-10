using Microsoft.Xna.Framework.Input;
using Nova.GUIEngine.Base;

namespace Nova.GUIEngine.Triggers
{
    public class ToggleVisibilityKeyboardTrigger : ITrigger
    {
        private readonly Panel _parent;
        private readonly Keys _key;
        private bool _isDown;

        public ToggleVisibilityKeyboardTrigger(Panel parent, Keys key)
        {
            _parent = parent;
            _key = key;
        }

        public void CheckTrigger()
        {
            bool previous = _isDown;
            _isDown = Keyboard.GetState().IsKeyDown(_key);

            if (previous && !_isDown)
                Execute();
            
            return;
        }

        private void Execute()
        {
            _parent.IsHidden = !_parent.IsHidden;
        }
    }
}
