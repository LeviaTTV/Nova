using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nova.Common.Sprite;

namespace Editor
{
    public class SpriteSheetViewModel
    {
        public ObservableCollection<Sprite> Sprites => new ObservableCollection<Sprite>(SpriteSheet.Sprites);
        public SpriteSheet SpriteSheet { get; set; }

    }
}
