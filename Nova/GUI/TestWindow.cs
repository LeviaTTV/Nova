using Nova.GUIEngine.Base;
using Nova.GUIEngine.Units;

namespace Nova.GUI
{
    public class TestWindow : Window
    {
        public TestWindow()
        {
            Size = new USize(UDim.Relative(0.5f));
            Position = new UVector2(UDim.Absolute(200), UDim.Absolute(50));

            Title = "MY BEAUTIJFUL TEST WINDOW!?";
            CanClose = true;
            Icon = "Inventory";
        }
    }
}
