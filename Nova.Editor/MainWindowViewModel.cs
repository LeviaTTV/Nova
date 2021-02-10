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
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Color = Microsoft.Xna.Framework.Color;
using IDrawable = Nova.Common.IDrawable;
using IUpdateable = Nova.Common.IUpdateable;
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

        private string _openFromImageFileName;

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

                var oldName = sprite.Name;
                sprite.Name = value;


                var sheet = _treeItems.FirstOrDefault().SpriteSheet;
                if (sheet != null && sheet is SpriteSheet spriteSheet)
                {
                    spriteSheet.Sprites.Remove(oldName);
                    spriteSheet.Sprites[value] = sprite;
                }

                UpdateSpriteCollection();
            }
        }

        public int SpriteX
        {
            get
            {
                return _drawable is Sprite ? ((Sprite)_drawable).X : 0;
            }
            set
            {
                var sprite = _drawable as Sprite;

                if (sprite == null)
                    return;

                sprite.X = value;

                UpdateSpriteCollection();
            }
        }

        public int SpriteY
        {
            get
            {
                return _drawable is Sprite ? ((Sprite)_drawable).Y : 0;
            }
            set
            {
                var sprite = _drawable as Sprite;

                if (sprite == null)
                    return;

                sprite.Y = value;

                UpdateSpriteCollection();
            }
        }

        public int SpriteWidth
        {
            get
            {
                return _drawable is Sprite ? ((Sprite)_drawable).Width : 0;
            }
            set
            {
                var sprite = _drawable as Sprite;

                if (sprite == null)
                    return;

                sprite.Width = value;
            }
        }

        public int SpriteHeight
        {
            get
            {
                return _drawable is Sprite ? ((Sprite)_drawable).Height : 0;
            }
            set
            {
                var sprite = _drawable as Sprite;

                if (sprite == null)
                    return;

                sprite.Height = value;
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

        private bool _drawNames;
        public bool DrawNames
        {
            get => _drawNames;
            set
            {
                _drawNames = value;
                OnPropertyChanged(nameof(DrawNames));
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
        public ICommand LoadDimensionsFromAssetCommand { get; set; }
        public ICommand ClearSpritesCommand { get; set; }

        private PrimitiveLine _line;
        public object _lock = new object();

        public MainWindowViewModel()
        {
            LoadFromImageCommand = new RelayCommand<object>(LoadFromImage);
            LoadFromAssetCommand = new RelayCommand<object>(LoadFromAsset);
            LoadDimensionsFromAssetCommand = new RelayCommand<object>(LoadDimensionsFromAsset);
            MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(MouseWheelEvent);
            GridDimensionEnterPressedCommand = new RelayCommand<string>(SetSpriteCount);
            TreatRowAsNewSheetCommand = new RelayCommand<bool>(TreatRowsAsNewSheet);
            SpriteSelectedCommand = new RelayCommand<object>(SpriteSelected);
            SaveCommand = new RelayCommand<object>(Save);
            GenerateNoiseCommand = new RelayCommand<object>(GenerateNoise);
            ClearSpritesCommand = new RelayCommand<object>(ClearSprites);
        }

        private void ClearSprites(object obj)
        {
            if (_drawable is SpriteSheet sheet)
            {
                sheet.Sprites.Clear();
                UpdateSpriteCollection();
            }
        }

        private void LoadDimensionsFromAsset(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!openFileDialog.ShowDialog().GetValueOrDefault())
                return;
            
            var asset = openFileDialog.FileName;

            var assetSerialization = new BinaryAssetSerialization();

            using (var fs = File.OpenRead(asset))
            {
                var assetObject = assetSerialization.Deserialize(GraphicsDevice, fs, asset);
                
                if (_drawable is SpriteSheet sheet && assetObject is SpriteSheet assetSheet)
                {
                    sheet.Sprites.Clear();

                    foreach (var key in assetSheet.Sprites)
                    {
                        sheet.Sprites[key.Key] = key.Value;
                    }

                    UpdateSpriteCollection();
                }
            }
        }

        private void SpriteSelected(object obj)
        {
            if (obj is IDrawable drawable)
            {
                _drawable = drawable;
                OnPropertyChanged("SpritePanelName");
                OnPropertyChanged("SpriteX");
                OnPropertyChanged("SpriteY");
                OnPropertyChanged("SpriteWidth");
                OnPropertyChanged("SpriteHeight");
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

            _openFromImageFileName = null;
            var asset = openFileDialog.FileName;

            var assetSerialization = new BinaryAssetSerialization();

            using (var fs = File.OpenRead(asset))
            {
                var assetObject = assetSerialization.Deserialize(GraphicsDevice, fs, asset);


                if (assetObject is AnimatedSpriteSheet animatedSheet)
                {
                    _drawable = animatedSheet;

                    Name = animatedSheet.Name;
                    _gridHeight = animatedSheet.Sprites.FirstOrDefault().Value.Height;
                    _gridWidth = animatedSheet.Sprites.FirstOrDefault().Value.Width;
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
                    _gridHeight = sheet.Sprites.FirstOrDefault().Value.Height;
                    _gridWidth = sheet.Sprites.FirstOrDefault().Value.Width;
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
                    
                    var assetSerialization = new BinaryAssetSerialization();

                    lock (_lock)
                        assetSerialization.Serialize(item, fs);

                    string name = Path.GetFileNameWithoutExtension(_openFromImageFileName);

                    if (_drawable is SpriteSheet sheet)
                        name = sheet.Name;

                    if (_openFromImageFileName != null)
                        File.Copy(_openFromImageFileName, Path.Combine(Path.GetDirectoryName(fileName), name + "Texture" + Path.GetExtension(_openFromImageFileName)), true);
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


            _line = new PrimitiveLine(GraphicsDevice, Color.Red);
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

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(_scale.X));

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

                    if (drawable is SpriteSheet sheet)
                    {
                        // Debug lines

                        foreach (var sprite in sheet.Sprites.Values)
                        {
                            _line.Draw(_spriteBatch, new Vector2(_position.X + sprite.X, _position.Y + sprite.Y), new Vector2(_position.X + sprite.X + sprite.Width, _position.Y + sprite.Y));
                            _line.Draw(_spriteBatch, new Vector2(_position.X + sprite.X, _position.Y + sprite.Y), new Vector2(_position.X + sprite.X, _position.Y + sprite.Y + sprite.Height));

                            _line.Draw(_spriteBatch, new Vector2(_position.X + sprite.X + sprite.Width, _position.Y + sprite.Y), new Vector2(_position.X + sprite.X + sprite.Width, _position.Y + sprite.Y + sprite.Height));
                            _line.Draw(_spriteBatch, new Vector2(_position.X + sprite.X, _position.Y + sprite.Y + sprite.Height), new Vector2(_position.X + sprite.X + sprite.Width, _position.Y + sprite.Y + sprite.Height));

                            if (_drawNames)
                                _spriteBatch.DrawString(_font, sprite.Name, new Vector2(_position.X + sprite.X + 2, _position.Y + sprite.Y + 2), Color.Blue);
                        }
                    }
                }
            }
            
            //
            if (_selectionRectangle != default(Rectangle))
            {
                var line = new PrimitiveLine(GraphicsDevice, Color.Green);

                line.Draw(_spriteBatch, new Vector2(_selectionRectangle.X, _selectionRectangle.Y), new Vector2(_selectionRectangle.X + _selectionRectangle.Width, _selectionRectangle.Y), 0.5f);
                line.Draw(_spriteBatch, new Vector2(_selectionRectangle.X + _selectionRectangle.Width, _selectionRectangle.Y), new Vector2(_selectionRectangle.X + _selectionRectangle.Width, _selectionRectangle.Y + _selectionRectangle.Height), 0.5f);
                line.Draw(_spriteBatch, new Vector2(_selectionRectangle.X, _selectionRectangle.Y + _selectionRectangle.Height), new Vector2(_selectionRectangle.X + _selectionRectangle.Width, _selectionRectangle.Y + _selectionRectangle.Height), 0.5f);
                line.Draw(_spriteBatch, new Vector2(_selectionRectangle.X, _selectionRectangle.Y), new Vector2(_selectionRectangle.X, _selectionRectangle.Y +_selectionRectangle.Height), 0.5f);

            }
            
            _spriteBatch.End();
        }

        private void LoadFromImage(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!openFileDialog.ShowDialog().GetValueOrDefault())
                return;
            
            _openFromImageFileName = openFileDialog.FileName;
            _texture = Texture2D.FromStream(GraphicsDevice, File.OpenRead(_openFromImageFileName));

            Name = Path.GetFileNameWithoutExtension(_openFromImageFileName);

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

                            sheet.Sprites[count.ToString()] = new Sprite()
                            {
                                Height = _gridHeight,
                                Width = _gridWidth,
                                X = zx * _gridWidth,
                                Y = zy * _gridHeight,
                                Texture = _texture,
                                Name = count.ToString()
                            };
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

                            sheet.Sprites[count.ToString()] = new Sprite()
                            {
                                Height = _gridHeight,
                                Width = _gridWidth,
                                X = zx * _gridWidth,
                                Y = zy * _gridHeight,
                                Texture = _texture,
                                Name = count.ToString()
                            };
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



            _viewportHeight = 400;
            _viewportWidth = 400;
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

        public void Select(double startPositionX, double startPositionY, double endPositionX, double endPositionY, bool addMode = false)
        {
            var transform = Matrix.Invert(Matrix.CreateScale(_scale.X));

            Vector2 start = Vector2.Transform(new Vector2((float)startPositionX, (float)startPositionY), transform);
            Vector2 end = Vector2.Transform(new Vector2((float)endPositionX, (float)endPositionY), transform);

            _selectionRectangle = default(Rectangle);

            if (_drawable is SpriteSheet sheet)
            {
                if (!addMode)
                {
                    var sprites = sheet.Sprites.Values.Where(x => _position.X + x.X + x.Width > start.X && _position.X + x.X <= end.X &&
                                                                  _position.Y + x.Y + x.Height > start.Y && _position.Y + x.Y <= end.Y).ToList();

                    if (sprites.Count() > 1)
                    {
                        var first = sprites.FirstOrDefault();

                        // New rect
                        first.Width = sprites.Max(x => x.X) + first.Width - first.X;
                        first.Height = sprites.Max(x => x.Y) + first.Height - first.Y;

                        // Remove others
                        foreach (var key in sprites)
                        {
                            if (key == first)
                                continue;

                            sheet.Sprites.Remove(key.Name);
                        }

                        UpdateSpriteCollection();
                    }
                }
                else
                {
                    var spriteName = "Sprite";

                    int count = 1;
                    while (sheet.Sprites.ContainsKey(spriteName))
                    {
                        spriteName += count;
                        ++count;
                    }

                    var sprite = new Sprite()
                    {
                        Height = (int)(end.Y - start.Y), 
                        Width = (int)(end.X - start.X),
                        X = (int)(start.X - _position.X),
                        Y = (int)(start.Y - _position.Y),
                        Name = spriteName,
                        Texture = sheet.Texture
                    };

                    sheet.Sprites[sprite.Name] = sprite;
                    UpdateSpriteCollection();
                }

            }
        }

        public Rectangle _selectionRectangle;

        public void TransformSelection(double startPositionX, double startPositionY, double relativePositionX, double relativePositionY)
        {
            var transform = Matrix.Invert(Matrix.CreateScale(_scale.X));

            Vector2 start = Vector2.Transform(new Vector2((float)startPositionX, (float)startPositionY), transform);
            Vector2 end = Vector2.Transform(new Vector2((float)relativePositionX, (float)relativePositionY), transform);

            _selectionRectangle = new Rectangle((int)start.X, (int)start.Y, (int)end.X - (int)start.X, (int)end.Y - (int)start.Y);
        }
    }
}