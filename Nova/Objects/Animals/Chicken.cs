using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Sprite;
using Nova.Objects.Character;

namespace Nova.Objects.Animals
{
    public class Chicken : LivingGameObject
    {
        private SpriteSheet _chickenEatSheet;
        private SpriteSheet _chickenWalkSheet;

        
        

        private readonly Dictionary<Orientation, AnimatedSpriteSheet> _walkingSheets = new Dictionary<Orientation, AnimatedSpriteSheet>();


        private AnimatedSpriteSheet _currentSpriteSheet;

        private bool _isMoving = false;

        public Chicken(GameServiceContainer services) : base(services)
        {
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            _chickenEatSheet = contentManager.Load<SpriteSheet>("Animals/AnimalChickenEat");
            _chickenWalkSheet = contentManager.Load<SpriteSheet>("Animals/AnimalChickenWalk");
            
            var sheet = new AnimatedSpriteSheet(_chickenWalkSheet.Texture, 32, 32);
            int count = 0;
            foreach (var sprite in _chickenWalkSheet.GetSprites(0, 4))
            {
                ++count;
                sheet.Sprites[count.ToString()] = sprite;
            }
            _walkingSheets[Orientation.Top] = sheet;

            sheet = new AnimatedSpriteSheet(_chickenWalkSheet.Texture, 32, 32);
            count = 0;
            foreach (var sprite in _chickenWalkSheet.GetSprites(4, 4))
            {
                ++count;
                sheet.Sprites[count.ToString()] = sprite;
            }
            _walkingSheets[Orientation.Left] = sheet;

            sheet = new AnimatedSpriteSheet(_chickenWalkSheet.Texture, 32, 32);
            count = 0;
            foreach (var sprite in _chickenWalkSheet.GetSprites(8, 4))
            {
                ++count;
                sheet.Sprites[count.ToString()] = sprite;
            }
            _walkingSheets[Orientation.Bottom] = sheet;

            sheet = new AnimatedSpriteSheet(_chickenWalkSheet.Texture, 32, 32);
            count = 0;
            foreach (var sprite in _chickenWalkSheet.GetSprites(12, 4))
            {
                ++count;
                sheet.Sprites[count.ToString()] = sprite;
            }
            _walkingSheets[Orientation.Right] = sheet;


            foreach (var tempSheet in _walkingSheets)
                tempSheet.Value.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _currentSpriteSheet = _walkingSheets[Orientation];
            
            if (!_isMoving)
                _currentSpriteSheet.Reset();
            else
                _currentSpriteSheet.Update(gameTime);

            _isMoving = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _currentSpriteSheet.Draw(spriteBatch, Position, layerDepth: 0f);
        }
    }
}
