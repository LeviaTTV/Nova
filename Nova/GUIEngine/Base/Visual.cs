using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Nova.GUIEngine.Units;

namespace Nova.GUIEngine.Base
{
    public abstract class Visual
    {
        public UVector2 Position { get; set; }
        public USize Size { get; set; }

        public List<Visual> Children { get; } = new List<Visual>();

        public UVector2 ActualPosition { get; set; }
        public USize ActualSize { get; set; }

        public List<Keys> InterestedKeys { get; set; } = new List<Keys>();
        
        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                _isHidden = value;

                foreach (var child in Children)
                    child.IsHidden = _isHidden;
            }
        }
        public Visual Parent { get; set; }
        
        public void Add(Visual child)
        {
            if (child.Parent != null)
                throw new ArgumentException("Visual cannot be a child of two parents, contrary to popular belief.");
            
            Children.Add(child);
            child.Parent = this;
        }

        public void Remove(Visual child)
        {
            if (Children.Remove(child))
                child.Parent = null;
        }

        public virtual void LoadContent(ContentManager content) {}

        public virtual void Draw(DrawingContext ctx) {}

        public virtual void OnLeftMouseDown(Vector2 position)
        {
        }

        public virtual void OnLeftMouseUp(Vector2 position)
        {
        }

        public virtual void OnInterestedKeyDown(Keys key)
        {
        }

        public virtual void OnInterestedKeyUp(Keys key)
        {
        }

    }
}
