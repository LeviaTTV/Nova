using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Nova.GUIEngine;
using Nova.GUIEngine.Base;
using Nova.GUIEngine.Elements;
using Nova.GUIEngine.Units;

namespace Nova.GUI
{
    /// <summary>
    /// A hotbar I guess. Needs to be changed to give slots their own classes etc.
    /// </summary>
    public class HotbarPanel : Panel
    {
        public int HotbarSlots { get; set; } = 10;

        private int _slotsInitialized;
        private ContentManager _contentManager;

        public int SelectedIndex { get; set; }

        public HotbarPanel() : base("PanelAlt1", false)
        {
            Size = new USize(UDim.Relative(0.2f), UDim.Absolute(41f));
            Position = new UVector2(UDim.Relative(0.4f), new UDim(1f, -41f));

            InterestedKeys.Add(Keys.D1);
            InterestedKeys.Add(Keys.D2);
            InterestedKeys.Add(Keys.D3);
            InterestedKeys.Add(Keys.D4);
            InterestedKeys.Add(Keys.D5);
            InterestedKeys.Add(Keys.D6);
            InterestedKeys.Add(Keys.D7);
            InterestedKeys.Add(Keys.D8);
            InterestedKeys.Add(Keys.D9);
            InterestedKeys.Add(Keys.D0);

            SelectedIndex = 1;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _contentManager = content;
        }
        
        private void ReinitializeSlots()
        {
            var basePosition = new UVector2(UDim.Absolute(3f));

            Children.Clear();
            for (int i = 0; i < HotbarSlots; i++)
            {
                Add(new ImageElement("InventorySlot")
                {
                    Position = basePosition + new UVector2(UDim.Absolute(i * 38f), UDim.Zero)
                });

                if (i == 0)
                    Add(new ImageElement("ShovelIcon")
                    {
                        Position = basePosition + new UVector2(UDim.Absolute(i * 38f + 2f), UDim.Absolute(1f))
                    });


                var text = (i + 1).ToString();
                if (i + 1 == 10)
                {
                    text = "0";
                }

                Add(new TextElement()
                {
                    Text = text,
                    Position = basePosition + new UVector2(UDim.Absolute(i * 38f + 3f), UDim.Absolute(1f)),
                    Color = (i == 0) ? Color.Yellow : Color.White
                });
            }

            foreach (var child in Children)
                child.LoadContent(_contentManager);

            _slotsInitialized = HotbarSlots;
        }

        public override void Draw(DrawingContext ctx)
        {
            if (_slotsInitialized != HotbarSlots)
                ReinitializeSlots();

            base.Draw(ctx);
        }
        
        public override void OnInterestedKeyUp(Keys key)
        {
            var part = key.ToString().Substring(1);

            foreach (var old in Children.OfType<TextElement>().Where(x => x.Text != part))
                old.Color = Color.White;

            var newElement = Children.OfType<TextElement>().FirstOrDefault(x => x.Text == part);
            newElement.Color = Color.Yellow;

            SelectedIndex = int.Parse(part);
        }
    }
}
