using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using Path = System.IO.Path;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace Nova.EdgeDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Load(Path.Combine(Directory.GetCurrentDirectory(), "../../testimage.png"));
        }

        private void Load(string dialogFileName)
        {
            var bitmap = new Bitmap(dialogFileName);
            OriginalImage.Source = BitmapToImageSource(bitmap);

            // The idea is to some basic edge detection based on alpha values so we could feed it as a polygon into the Penumbra lighting system for hulls. Sadly,
            //  the idea was abandoned because Penumbra is not made for hulls with 2000 point polygons and performance will suffer. Also, the method is not very 
            //  accurate. I'm keeping the project around for posterity reasons or..something.
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var index = 0;
            var bytes = new byte[data.Height * data.Stride];

            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);


            var results = new List<OutlineEntry>();
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    byte a = bytes[index + 3];
                    byte r = bytes[index + 2];
                    byte g = bytes[index + 1];
                    byte b = bytes[index];

                    if (a > 60)
                    {
                        results.Add(new OutlineEntry()
                        {
                            X = x,
                            Y = y
                        });
                    }

                    index += 4;
                }
            }

            bitmap.UnlockBits(data);

            var newResultList = new List<OutlineEntry>();
            foreach (var outlineEntry in results)
            {
                if (outlineEntry.X == 46 && outlineEntry.Y == 2)
                    outlineEntry.ToString();
                int count = 0;
                if (results.Any(z => outlineEntry.X == z.X - 1 && outlineEntry.Y == z.Y))
                    ++count;
                if (results.Any(z => outlineEntry.X == z.X + 1 && outlineEntry.Y == z.Y))
                    ++count;
                if (results.Any(z => outlineEntry.X == z.X && outlineEntry.Y == z.Y - 1))
                    ++count;
                if (results.Any(z => outlineEntry.X == z.X && outlineEntry.Y == z.Y + 1))
                    ++count;

                if (count <= 3)
                    newResultList.Add(outlineEntry);
            }



            var outputImage = new Bitmap(bitmap.Width, bitmap.Height);
            var graphics = Graphics.FromImage(outputImage);
            graphics.SmoothingMode = SmoothingMode.None;
            

            foreach (var res in newResultList)
            {
                graphics.FillRectangle(Brushes.Black, res.X, res.Y, 1, 1);
            }

            graphics.Dispose();

            outputImage.Save("dsadlkaslkdsa.png", ImageFormat.Png);

            EdgeDetectedImage.Source = BitmapToImageSource(outputImage);


            var str = JsonConvert.SerializeObject(newResultList, Formatting.None);
            File.WriteAllText("tree.json", str);


        }
        
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

    }

    public struct OutlineEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
