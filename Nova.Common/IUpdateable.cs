using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nova.Common
{
    public interface IUpdateable
    {
        void Update(GameTime gameTime);
    }
}
