using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Nova.Common.Sprite
{
    public class AnimatedSpriteSheet : SpriteSheet, IUpdateable
    {
        public long StartDelay { get; set; } = 0;
        public long TimeBetweenSprites { get; set; } = 83;
        public bool Repeat { get; set; } = true;
        public bool ReverseAtEnd { get; set; }

        [JsonIgnore]
        public int CurrentIndex { get; private set; } = 0;

        [JsonIgnore]
        public int SpriteCount => Sprites.Count;

        [JsonIgnore]
        public bool HasEnded { get; private set; }

        [JsonIgnore]
        private bool _running = false;

        [JsonIgnore]
        private double _accumulator;

        public AnimatedSpriteSheet()
        {
        }

        public AnimatedSpriteSheet(Texture2D texture, int width, int height)
        {
            Texture = texture;
            Width = width;
            Height = height;
        }

        public void Stop()
        {
            _running = false;
            HasEnded = true;
        }

        public void Start()
        {
            _running = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!_running)
                return;

            _accumulator += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (CurrentIndex == 0 && _accumulator < StartDelay)
                return;

            if (_accumulator > TimeBetweenSprites)
            {
                ++CurrentIndex;
                _accumulator = 0;
            }

            if (CurrentIndex == SpriteCount)
            {
                if (Repeat)
                {
                    CurrentIndex = 0;
                    if (ReverseAtEnd)
                        Sprites.Reverse();
                }
                else
                    Stop();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (HasEnded)
                return;

            _running = true;

            var sprite = Sprites[CurrentIndex];
            sprite.Draw(spriteBatch, position);
        }
    }
}
