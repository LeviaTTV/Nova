using System.Collections.ObjectModel;
using Nova.Common.Sprite;

namespace Editor
{
    public class SpriteSheetViewModel
    {
        public ObservableCollection<Sprite> Sprites => new ObservableCollection<Sprite>(SpriteSheet.Sprites.Values);
        public SpriteSheet SpriteSheet { get; set; }

    }
}
