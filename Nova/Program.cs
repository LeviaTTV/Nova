using System;

namespace Nova
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new NovaGame())
                game.Run();
        }
    }
}
