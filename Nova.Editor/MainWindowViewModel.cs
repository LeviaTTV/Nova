using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Editor.MonoGameControls;
using EpPathFinding.cs;
using Microsoft.Win32;
using Nova.Common.Noise;
using Nova.Common.Sprite;
using Color = Microsoft.Xna.Framework.Color;
using IDrawable = Nova.Common.Sprite.IDrawable;
using IUpdateable = Nova.Common.Sprite.IUpdateable;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Editor
{
    public class MainWindowViewModel : MonoGameViewModel
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _position;
        private Vector2 _origin;
        private Vector2 _scale;
        private SpriteFont _font;

        private int _gridHeight = 32;
        private int _gridWidth = 32;

        private IDrawable _drawable;

        private string _name;
        private bool _isAnimation;
        private bool _reverseAtEnd;
        private int _timeBetweenSprites = 83;
        private int _animationStartDelay = 0;

        private int _activeTab = 0;

        public Visibility SpritePanelVisibility => _drawable is Sprite ? Visibility.Visible : Visibility.Hidden;

        public string SpritePanelName
        {
            get { return _drawable is Sprite ? ((Sprite) _drawable).Name : ""; }
            set
            {
                var sprite = _drawable as Sprite;

                if (sprite == null)
                    return;

                sprite.Name = value;

                UpdateSpriteCollection();
            }
        }

        private ObservableCollection<SpriteSheetViewModel> _treeItems = new ObservableCollection<SpriteSheetViewModel>();
        public ObservableCollection<SpriteSheetViewModel> TreeItems => _treeItems;

        public bool IsAnimation
        {
            get => _isAnimation;
            set
            {
                _isAnimation = value;
                OnPropertyChanged(nameof(IsAnimation));

                SpriteSheetChanged();
            }
        }

        public bool ReverseAtEnd
        {
            get => _reverseAtEnd;
            set
            {
                _reverseAtEnd = value;
                OnPropertyChanged(nameof(ReverseAtEnd));

                SpriteSheetChanged();
            }
        }


        public int AnimationStartDelay
        {
            get => _animationStartDelay;
            set
            {
                _animationStartDelay = value;
                OnPropertyChanged(nameof(IsAnimation));

                SpriteSheetChanged();
            }
        }

        public int TimeBetweenSprites
        {
            get => _timeBetweenSprites;
            set
            {
                _timeBetweenSprites = value;
                OnPropertyChanged(nameof(IsAnimation));

                SpriteSheetChanged();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (_drawable is SpriteSheet spriteSheet)
                    spriteSheet.Name = value;

                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _treatRowsAsNewSheet;

        public ICommand LoadFromImageCommand { get; set; }
        public ICommand LoadFromAssetCommand { get; set; }
        public ICommand MouseWheelCommand { get; set; }
        public ICommand TreatRowAsNewSheetCommand { get; set; }
        public ICommand SpriteSelectedCommand { get; set; } 
        public ICommand GridDimensionEnterPressedCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand GenerateNoiseCommand { get; set; }

        public object _lock = new object();

        public MainWindowViewModel()
        {
            LoadFromImageCommand = new RelayCommand<object>((obj) => LoadFromImage());
            LoadFromAssetCommand = new RelayCommand<object>(LoadFromAsset);
            MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(MouseWheelEvent);
            GridDimensionEnterPressedCommand = new RelayCommand<string>(SetSpriteCount);
            TreatRowAsNewSheetCommand = new RelayCommand<bool>(TreatRowsAsNewSheet);
            SpriteSelectedCommand = new RelayCommand<object>(SpriteSelected);
            SaveCommand = new RelayCommand<object>(Save);
            GenerateNoiseCommand = new RelayCommand<object>(GenerateNoise);
        }

        private void SpriteSelected(object obj)
        {
            if (obj is IDrawable drawable)
            {
                _drawable = drawable;
                OnPropertyChanged("SpritePanelName");
            }
            else if (obj is SpriteSheetViewModel vm)
                _drawable = vm.SpriteSheet;

            OnPropertyChanged("SpritePanelVisibility");
        }

        private void LoadFromAsset(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!openFileDialog.ShowDialog().GetValueOrDefault())
                return;

            var asset = openFileDialog.FileName;

            var assetSerialization = new BinaryAssetSerialization();
            //var assetSerialization = new JsonAssetSerialization(Path.GetDirectoryName(asset));

            using (var fs = File.OpenRead(asset))
            {
                var assetObject = assetSerialization.Deserialize(GraphicsDevice, File.OpenRead(asset));


                if (assetObject is AnimatedSpriteSheet animatedSheet)
                {
                    _drawable = animatedSheet;

                    Name = animatedSheet.Name;
                    _gridHeight = animatedSheet.Sprites.FirstOrDefault().Height;
                    _gridWidth = animatedSheet.Sprites.FirstOrDefault().Width;
                    _texture = animatedSheet.Texture;
                    IsAnimation = true;
                    ReverseAtEnd = animatedSheet.ReverseAtEnd;
                    TimeBetweenSprites = (int)animatedSheet.TimeBetweenSprites;
                    AnimationStartDelay = (int)animatedSheet.StartDelay;

                }
                else if (assetObject is SpriteSheet sheet)
                {
                    _drawable = sheet;

                    Name = sheet.Name;
                    _gridHeight = sheet.Sprites.FirstOrDefault().Height;
                    _gridWidth = sheet.Sprites.FirstOrDefault().Width;
                    _texture = sheet.Texture;
                }

            }

            UpdateSpriteCollection();
        }

        private void Save(object obj)
        {
            if (_drawable == null)
                return;

            SaveFileDialog dialog = new SaveFileDialog();

            if (_drawable is AnimatedSpriteSheet)
            {
                dialog.DefaultExt = ".nvas";
                dialog.FileName = _name + ".nvas";
                dialog.Filter = "Nova Animated Sprite Sheet (.nvas)|*.nvas";
            }
            else if (_drawable is SpriteSheet)
            {
                dialog.DefaultExt = ".nvs";
                dialog.FileName = _name + ".nvs";
                dialog.Filter = "Nova Sprite Sheet (.nvs)|*.nvs";
            }

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                SaveImpl(dialog.FileName);
            }
        }

        private void SaveImpl(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                if (_drawable != null)
                {
                    var item = _drawable;

                    if (item is Sprite)
                        item = _treeItems.FirstOrDefault().SpriteSheet;

                    //var assetSerialization = new JsonAssetSerialization(Path.GetDirectoryName(fileName));
                    var assetSerialization = new BinaryAssetSerialization();

                    lock (_lock)
                        assetSerialization.Serialize(item, fs);
                }
            }
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = Content.Load<Texture2D>("monogame-logo");
            _scale = Vector2.One;
            _position = GraphicsDevice.Viewport.Bounds.Center.ToVector2();

            _font = Content.Load<SpriteFont>("font");



            _gridLineTexture2D = new Texture2D(GraphicsDevice, 1, 1);
            _gridLineTexture2D.SetData(new Color[] { Color.White });

            SpriteSheetChanged();
        }

        private Texture2D _gridLineTexture2D;

        public override void Update(GameTime gameTime)
        {
            _origin = _texture.Bounds.Center.ToVector2();

            if (_position == Vector2.Zero)
            {
                _position = GraphicsDevice.Viewport.Bounds.Center.ToVector2() - _origin;
            }


            if (_drawable != null && _drawable is IUpdateable updateable)
            {
                lock (_lock)
                {
                    updateable.Update(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Matrix.CreateScale(_scale.X));

            if (_activeTab == 1)
            {
                if (_viewportHeight == 0 || _viewportHeight == 0)
                {
                    _viewportHeight = GraphicsDevice.Viewport.Bounds.Height;
                    _viewportWidth = GraphicsDevice.Viewport.Bounds.Width;
                }
                if (_noiseTexture != null)
                {
                    lock (_lock)
                        _spriteBatch.Draw(_noiseTexture, _position, Color.White);


                    if (_pathFindingList != null && _pathFindingList.Any())
                    {
                        for (int i = 0; i < _pathFindingList.Count; i++)
                        {
                            if (i + 1 == _pathFindingList.Count) 
                                break;

                            var next = _pathFindingList[i + 1];
                            var cur = _pathFindingList[i];

                            var start = new Vector2((float)cur.x, (float)cur.y) + _position;
                            var end = new Vector2((float)next.x, (float)next.y) + _position;

                            _spriteBatch.Draw(_gridLineTexture2D, start, null, Color.Red,
                                (float)Math.Atan2(end.Y - start.Y, end.X - start.X),
                                new Vector2(0f, (float)_gridLineTexture2D.Height / 2),
                                new Vector2(Vector2.Distance(start, end), 1f),
                                SpriteEffects.None, 0f);
                        }
                    }
                }

                _spriteBatch.End();
                return;
            }

            int x = (int)_position.X;
            int y = (int)_position.Y;

            if (_drawable != null && _drawable is IDrawable drawable)
            {
                lock (_lock)
                {
                    drawable.Draw(_spriteBatch, _position);
                }
            }

            if (!(_drawable is AnimatedSpriteSheet) && !(_drawable is Sprite))
            {
                int texWidth = _texture.Width;
                int texHeight = _texture.Height;

                int rows = texHeight / _gridHeight;
                int columns = texWidth / _gridWidth;

                for (int i = 0; i != rows + 1; ++i)
                    _spriteBatch.Draw(_gridLineTexture2D, new Rectangle(x, y + i * _gridHeight, texWidth, 1), Color.Red);

                for (int i = 0; i != columns + 1; ++i)
                    _spriteBatch.Draw(_gridLineTexture2D, new Rectangle(x + i * _gridWidth, y, 1, texHeight), Color.Red);

                int count = 0;
                for (int zy = 0; zy != rows; ++zy)
                {
                    if (_treatRowsAsNewSheet)
                        count = 0;
                    for (int zx = 0; zx != columns; ++zx)
                    {
                        ++count;
                        int startX = x + zx * _gridWidth;
                        int startY = y + zy * _gridHeight;

                        _spriteBatch.DrawString(_font, count.ToString(), new Vector2(startX + 2, startY + 2), Color.Black);
                    }
                }
            }
            
            _spriteBatch.End();
        }

        private void LoadFromImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!openFileDialog.ShowDialog().GetValueOrDefault())
                return;

            var image = openFileDialog.FileName;
            _texture = Texture2D.FromStream(GraphicsDevice, File.OpenRead(image));

            Name = Path.GetFileNameWithoutExtension(image);

            SpriteSheetChanged();
        }

        public void SetSpriteCount(string args)
        {
            int dimensions = int.Parse(args);
            _gridHeight = dimensions;
            _gridWidth = dimensions;

            SpriteSheetChanged();
        }

        public void MouseWheelEvent(MouseWheelEventArgs param)
        {
            if (param.Delta > 0)
            {
                _scale *= 1.05f;
            }
            else
            {
                _scale /= 1.05f;
            }
        }

        private void SpriteSheetChanged()
        {
            lock (_lock)
            {
                if (!_isAnimation)
                {
                    var sheet = new SpriteSheet()
                    {
                        Texture = _texture,
                        Name = Name
                    };

                    int x = (int)_position.X - (int)_origin.X;
                    int y = (int)_position.Y - (int)_origin.Y;

                    int texWidth = _texture.Width;
                    int texHeight = _texture.Height;

                    int rows = texHeight / _gridHeight;
                    int columns = texWidth / _gridWidth;

                    int count = 0;
                    for (int zy = 0; zy != rows; ++zy)
                    {
                        for (int zx = 0; zx != columns; ++zx)
                        {
                            ++count;

                            int startX = x + zx * _gridWidth;
                            int startY = y + zy * _gridHeight;

                            sheet.Sprites.Add(new Sprite()
                            {
                                Height = _gridHeight,
                                Width = _gridWidth,
                                X = zx * _gridWidth,
                                Y = zy * _gridHeight,
                                Texture = _texture,
                                Name = count.ToString()
                            });
                        }
                    }

                    _drawable = sheet;
                }
                else
                {
                    var sheet = new AnimatedSpriteSheet(_texture, _gridWidth, _gridHeight)
                    {
                        StartDelay = _animationStartDelay,
                        TimeBetweenSprites = _timeBetweenSprites,
                        ReverseAtEnd = _reverseAtEnd,
                        Name = Name
                    };

                    int x = (int) _position.X - (int) _origin.X;
                    int y = (int) _position.Y - (int) _origin.Y;

                    int texWidth = _texture.Width;
                    int texHeight = _texture.Height;

                    int rows = texHeight / _gridHeight;
                    int columns = texWidth / _gridWidth;

                    int count = 0;
                    for (int zy = 0; zy != rows; ++zy)
                    {
                        for (int zx = 0; zx != columns; ++zx)
                        {

                            ++count;
                            int startX = x + zx * _gridWidth;
                            int startY = y + zy * _gridHeight;

                            sheet.Sprites.Add(new Sprite()
                            {
                                Height = _gridHeight,
                                Width = _gridWidth,
                                X = zx * _gridWidth,
                                Y = zy * _gridHeight,
                                Texture = _texture,
                                Name = count.ToString()
                            });
                        }
                    }

                    _drawable = sheet;
                }


                UpdateSpriteCollection();
            }
        }

        private void UpdateSpriteCollection()
        {
            var item = _drawable;
            if (item is Sprite)
                item = _treeItems.FirstOrDefault().SpriteSheet;

            _treeItems.Clear();


            if (item is SpriteSheet sheet)
            {
                _treeItems.Add(new SpriteSheetViewModel()
                {
                    SpriteSheet = sheet
                });
            }
        }

        private void TreatRowsAsNewSheet(bool obj)
        {
            _treatRowsAsNewSheet = obj;
        }

        public void Move(double deltaX, double deltaY)
        {
            _position.X += (float)deltaX;
            _position.Y += (float)deltaY;
        }

        public void ActivateSpriteTab()
        {
            _activeTab = 0;
        }

        public void ActiveNoiseTab()
        {
            _activeTab = 1;
        }


        //
        private Texture2D _noiseTexture;
        private bool _perlinNoiseSelected;
        public bool PerlinNoiseSelected
        {
            get => _perlinNoiseSelected;
            set
            {
                _perlinNoiseSelected = value;
                OnPropertyChanged(nameof(PerlinNoiseSelected));
            }
        }

        private bool _simplexNoiseSelected;
        public bool SimplexNoiseSelected
        {
            get => _simplexNoiseSelected;
            set
            {
                _simplexNoiseSelected = value;
                OnPropertyChanged(nameof(SimplexNoiseSelected));
            }
        }


        private bool _simplexSNoiseSelected;
        public bool SimplexSNoiseSelected
        {
            get => _simplexSNoiseSelected;
            set
            {
                _simplexSNoiseSelected = value;
                OnPropertyChanged(nameof(SimplexSNoiseSelected));
            }
        }


        private bool _valueNoiseSelected;
        public bool ValueNoiseSelected
        {
            get => _valueNoiseSelected;
            set
            {
                _valueNoiseSelected = value;
                OnPropertyChanged(nameof(ValueNoiseSelected));
            }
        }



        private bool _valueCubicNoiseSelected;
        public bool ValueCubicNoiseSelected
        {
            get => _valueCubicNoiseSelected;
            set
            {
                _valueCubicNoiseSelected = value;
                OnPropertyChanged(nameof(ValueCubicNoiseSelected));
            }
        }



        private bool _cellularNoiseSelected;
        public bool CellularNoiseSelected
        {
            get => _cellularNoiseSelected;
            set
            {
                _cellularNoiseSelected = value;
                OnPropertyChanged(nameof(CellularNoiseSelected));
            }
        }

        public string Seed { get; set; } = "333333";
        private int _viewportWidth, _viewportHeight;
        private List<GridPos> _pathFindingList;

        private void GenerateNoise(object obj)
        {
            INoise noise = null;
            if (_simplexNoiseSelected)
                noise = new FastNoise(FastNoiseLite.NoiseType.OpenSimplex2);
            else if (_simplexSNoiseSelected)
                noise = new FastNoise(FastNoiseLite.NoiseType.OpenSimplex2S);
            else if (_cellularNoiseSelected)
                noise = new FastNoise(FastNoiseLite.NoiseType.Cellular);
            else if (_valueNoiseSelected)
                noise = new FastNoise(FastNoiseLite.NoiseType.Value);
            else if (_valueCubicNoiseSelected)
                noise = new FastNoise(FastNoiseLite.NoiseType.ValueCubic);
            else if (_perlinNoiseSelected)
                noise = new PerlinNoise();



            var data = noise.Generate(int.Parse(Seed), _viewportWidth, _viewportHeight);


            var texture = new Texture2D(GraphicsDevice, _viewportWidth, _viewportHeight);

            var colorData = new Color[_viewportWidth * _viewportHeight];

            BaseGrid grid = new StaticGrid(_viewportWidth, _viewportHeight);

            long count = 0;
            for (int y = 0; y < _viewportHeight; y++)
            {
                for (int x = 0; x < _viewportWidth; x++)
                {
                    byte r = 0, g = 0, b = 0;
                    float v = data[x, y];

                    bool canTraverse = true;
                    if (v >= 0f && v < 0.20f)
                    {
                        b = 255;
                        canTraverse = false;
                    }
                    else if (v >= 0.1f && v < 0.2f)
                        g = 84;
                    else if (v >= 0.2f && v < 0.3f)
                        g = 105;
                    else if (v >= 0.3f && v < 0.4f)
                    {
                        g = 155;
                    }
                    else if (v >= 0.4f && v < 0.5f)
                    {
                        g = 255;
                    }
                    else if (v >= 0.5f && v < 0.6f)
                    {
                        r = 124;
                        g = 143;
                        b = 124;

                    }
                    else if (v >= 0.6f && v < 0.7f)
                    {
                        r = 109;
                        g = 128;
                        b = 102;
                        canTraverse = false;
                    }
                    else if (v >= 0.7f && v < 0.8f)
                    {
                        canTraverse = false;
                        r = 80;
                        g = 80;
                        b = 80;
                    }
                    else if (v >= 0.8f && v < 0.98f)
                    {
                        canTraverse = false;
                        r = 40;
                        g = 40;
                        b = 40;
                    }
                    else if (v >= 0.98f)
                    {
                        canTraverse = false;
                        r = 0;
                        g = 0;
                        b = 0;
                    }

                    colorData[count] = new Color(r, g, b, (byte)255);

                    grid.SetWalkableAt(x, y, canTraverse);

                    ++count;
                }
            }

            GridPos start = new GridPos(1, 1);
            GridPos end = new GridPos(_viewportWidth - 5, _viewportHeight - 5);

            var param = new JumpPointParam(grid, start, end, EndNodeUnWalkableTreatment.ALLOW);

            var list = JumpPointFinder.FindPath(param);

            _pathFindingList = list;

            texture.SetData<Microsoft.Xna.Framework.Color>(colorData);
            lock (_lock)
                _noiseTexture = texture;
        }

    }
}