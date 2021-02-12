using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Nova.Common.Sprite;

namespace Nova.Objects.Character
{
    public class AnimationSet
    {
        public string Name { get; set; }
        public List<Animation> Animations = new List<Animation>();

        public IEnumerable<AnimatedSpriteSheet> GetAnimatedSheetsForOrientation(Orientation orientation)
        {
            return Animations.Where(x => x.Orientation == orientation).SelectMany(x => x.AnimatedSpriteSheets);
        }

        public Animation GetAnimationForOrientation(Orientation orientation)
        {
            return Animations.FirstOrDefault(x => x.Orientation == orientation);
        }
    }

    public class Animation
    {
        public Orientation Orientation { get; set; }
        public List<AnimatedSpriteSheet> AnimatedSpriteSheets { get; set; } = new List<AnimatedSpriteSheet>();
    }

    public class AnimationSetBuilder
    {
        private readonly ContentManager _contentManager;
        private List<string> _assetNames = new List<string>();
        private int _frames;
        private Dictionary<Orientation, int> _indexes = new Dictionary<Orientation, int>();
        private string _name;
        private Action<AnimatedSpriteSheet> _optionsAction;

        public AnimationSetBuilder(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public static AnimationSetBuilder WithContentManager(ContentManager contentManager)
        {
            return new AnimationSetBuilder(contentManager);
        }

        public AnimationSetBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public AnimationSetBuilder WithAsset(string assetName)
        {
            _assetNames.Add(assetName);
            return this;
        }

        public AnimationSetBuilder WithFrameCount(int frames)
        {
            _frames = frames;
            return this;
        }

        public AnimationSetBuilder WithIndex(Orientation orientation, int index)
        {
            _indexes[orientation] = index;
            return this;
        }

        public AnimationSetBuilder WithAnimatedSpriteSheetOptions(Action<AnimatedSpriteSheet> opts)
        {
            _optionsAction = opts;
            return this;
        }

        public AnimationSet Build()
        {
            var ls = new AnimationSet()
            {
                Name = _name
            };

            ls.Animations.Add(new Animation()
            {
                Orientation = Orientation.Left
            });
            ls.Animations.Add(new Animation()
            {
                Orientation = Orientation.Top
            });
            ls.Animations.Add(new Animation()
            {
                Orientation = Orientation.Bottom
            });
            ls.Animations.Add(new Animation()
            {
                Orientation = Orientation.Right
            });


            foreach (var assetName in _assetNames)
            {
                var asset = _contentManager.Load<SpriteSheet>(assetName);
                

                foreach (var orientationEntry in _indexes)
                {
                    var sprites = asset.GetSprites(orientationEntry.Value, _frames);
                    var anim = new AnimatedSpriteSheet(asset.Texture, asset.Width, asset.Height);

                    foreach (var sp in sprites)
                        anim.Sprites[sp.Name] = sp;

                    _optionsAction?.Invoke(anim);

                    ls.Animations.FirstOrDefault(x => x.Orientation == orientationEntry.Key).AnimatedSpriteSheets.Add(anim);
                }
            }

            return ls;
        }
    }
}
