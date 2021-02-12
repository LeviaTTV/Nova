using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace Nova.Common.Extensions
{
    public static class ContentManagerExtensions
    {
        public static T LoadObject<T>(this ContentManager content, string name)
        {
            var obj = content.Load<JObject>(name);

            return obj.ToObject<T>();
        }

        public static SpriteFont LoadFont(this ContentManager content, string name)
        {
            var texture = content.Load<Texture2D>(name);

            var charList = new List<char>()
            {
                'A',
                'B',
                'C',
                'D',
                'E',
                'F',
                'G',
                'H',
                'I',
                'J',
                'K',
                'L',
                'M',
                'N',
                'O',
                'P',
                'Q',
                'R',
                'S',
                'T',
                'U',
                'V',
                'W',
                'X',
                'Y',
                'Z',

                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                '0',

                '-',
                '+',
                ':',
                '"',
                '.',
                '?',
                '!',
                '%',
                '=',
                '/',
                ' '
            };

            var boundList = new List<Rectangle>();
            var croppingList = new List<Rectangle>(new Rectangle[charList.Count]);
            var kerningList = new List<Vector3>();
            
            int count = 0;
            var dict = new Dictionary<char, Rectangle>();
            for (int y = 0; y < 10; y++)
            {
                int endX = 5;
                if (y == 5)
                    endX = 1;

                for (int x = 0; x < endX; x++)
                {
                    char c = charList[count];
                    ++count;
                    int width = 10;
                    if (y > 5)
                        width = 8;

                    int leftPadding = 5;
                    if (x == 0)
                        leftPadding = 0;

                    int topPadding = 5;
                    if (y == 0)
                        topPadding = 0;

                    if (c == 'I' || c == 'J')
                        width = 8;

                    if (c == '!')
                    {
                        width = 4;
                    }

                    dict[c] = new Rectangle(x * (10 + leftPadding), y * (10 + topPadding), width, 10);
                }
            }

            dict[' '] = new Rectangle(15, 75, 5, 10);

            foreach (var entry in dict.OrderBy(x => x.Key))
            {
                boundList.Add(entry.Value);

                var kerning = Vector3.Zero;
                if (entry.Key == 'I' || entry.Key == 'J')
                    kerning = new Vector3(0, 0, -2);
                else if (entry.Key == ' ')
                    kerning = new Vector3(0, 0, -6);
                else if (entry.Key == '1')
                    kerning = new Vector3(0, 0, -6);
                else if (entry.Key == ':')
                    kerning = new Vector3(1, 0, -4);
                else if (entry.Key == '!')
                    kerning = new Vector3(0, 0, -6);
                else if (int.TryParse(entry.Key.ToString(), out _))
                    kerning = new Vector3(0, 0, -2);

                kerningList.Add(kerning);
            }

            charList = charList.OrderBy(x => x).ToList();

            return new SpriteFont(texture, boundList, croppingList, charList, 12, 10, kerningList, '?');
        }
    }
}
