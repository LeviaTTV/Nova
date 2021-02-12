using System;
using Microsoft.Xna.Framework;

namespace Nova.Objects.Character
{
    public class LivingAction
    {
        public Action<GameTime, LivingAction> Action { get; set; }

        public bool IsRunning { get; set; }
    }
}
