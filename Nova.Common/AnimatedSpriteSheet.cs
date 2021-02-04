﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private List<Sprite> _sprites;

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
            _sprites = Sprites.Values.ToList();
        }

        public void Update(GameTime gameTime)
        {
            if (!_running)
                return;

            if (_sprites == null && Sprites.Any())
                _sprites = Sprites.Values.ToList();

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
                        _sprites.Reverse();
                }
                else
                    Stop();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = default(float))
        {
            if (HasEnded)
                return;

            _running = true;

            if (_sprites == null || !_sprites.Any())
                return;

            var sprite = _sprites[CurrentIndex];
            sprite.Draw(spriteBatch, position);
        }
    }
}
