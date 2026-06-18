using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    public sealed class UITileMenu : UIComponent
    {
        private readonly HashSet<UITile> _tiles = new HashSet<UITile>();

        private readonly float _scrollBarWidth = 2.5f;

        private float _horizontalGap;
        private float _verticalGap;

        private int _rows;
        private int _culomns;
        private int _pages;

        private UITile[,] _tileMap;
        private UITile _selectedTile;
        private Vec2 _selectionBoxPosition;

        private float _verticalTranslation;
        private float _tileWidth = 50f;
        private float _tileHeight = 30f;

        private int _currentCulomn;
        private int _currentRow;
        private int _currentPage;

        private bool _bindBoxPosition;

        public UITileMenu(float width, float height) : base(0f, 0f, width, height)
        {
            
        }

        public UITileMenu(IEnumerable<UITile> tiles, float width, float height) : base(0f, 0f, width, height)
        {
            _tiles = tiles.ToHashSet();
            ResetTileMap();
        }

        public float TileWidth
        {
            get
            {
                return _tileWidth;
            }

            set
            {
                if (_tileWidth != value)
                {
                    _tileWidth = value;
                    ResetTileMap();
                }
            }
        }

        public float TileHeight
        {
            get
            {
                return _tileHeight;
            }

            set
            {
                if (_tileHeight != value)
                {
                    _tileHeight = value;
                    ResetTileMap();
                }
            }
        }

        private Vec2 Position
        {
            get
            {
                if (parent is null)
                    return position;

                return position - new Vec2(parent.width / 2f, height / 2f);
            }
        }

        private bool Enabled => _pages > 0 && open;

        public override void Update()
        {
            base.Update();

            if (!Enabled)
                return;

            int culomn = _currentCulomn;
            int row = _currentRow;
            int page = _currentPage;

            if (Input.Pressed("MENURIGHT"))
            {
                culomn = Math.Min(_currentCulomn + 1, _culomns - 1);
                PlayTileChangeSound();
            }
            else if (Input.Pressed("MENULEFT"))
            {
                culomn = Math.Max(_currentCulomn - 1, 0);
                PlayTileChangeSound();
            }
            else if (Input.Pressed("MENUUP"))
            {
                if (_currentRow > 0)
                {
                    row--;
                }
                else
                {
                    if (_currentPage > 0)
                    {
                        page--;
                        row = _rows - 1;
                    }
                }

                PlayTileChangeSound();
            }
            else if (Input.Pressed("MENUDOWN"))
            {
                if (_currentRow < _rows - 1)
                {
                    row++;
                }
                else
                {
                    if (_currentPage < _pages - 1)
                    {
                        page++;
                        row = 0;
                    }
                }

                PlayTileChangeSound();
            }

            UITile selectedTile = GetTile(row, culomn, page);


            if (selectedTile is null && _currentPage != page)
            {
                culomn = 0;
                selectedTile = GetTile(row, culomn, page); 
            }

            if (selectedTile is not null)
            {
                _currentRow = row;
                _currentCulomn = culomn;

                _selectedTile = selectedTile;

                if (_currentPage != page)
                {
                    _verticalTranslation += height * (_currentPage - page);
                    _bindBoxPosition = true;

                    _currentPage = page;
                }
            }

            if (_selectedTile is not null)
            {
                Vec2 destination = _selectedTile.Position;

                _selectionBoxPosition = Lerp.Vec2(_selectionBoxPosition, destination, Vec2.Distance(_selectionBoxPosition, destination) / 4f);

                if (Input.Pressed("SELECT"))
                {
                    _selectedTile.OnClick();
                    SFX.Play("rockHitGround", 0.7f);
                }
            }

            for (int p = 0; p < _pages; p++)
            {
                for (int r = 0; r < _rows; r++)
                {
                    for (int c = 0; c < _culomns; c++)
                    {
                        UITile tile = _tileMap[c, p * _rows + r];

                        if (tile is null)
                            continue;

                        tile.Position = Position + new Vec2((TileWidth + _horizontalGap) * c + _horizontalGap, height * p + (TileHeight + _verticalGap) * r + _verticalGap + _verticalTranslation);
                    }
                }
            }

            if (_bindBoxPosition || animating)
            {
                if (_selectedTile is not null)
                    _selectionBoxPosition = _selectedTile.Position; 

                _bindBoxPosition = false;
            }
        }

        public override void Draw()
        {
            if (!Enabled)
                return;

            for (int row = 0; row < _rows; row++)
            {
                for (int culomn = 0; culomn < _culomns; culomn++)
                {
                    UITile tile = GetTile(row, culomn, _currentPage);

                    if (tile is not null)
                        tile.Draw();
                }
            }

            if (_selectedTile is not null && !animating)
                Graphics.DrawRect(new Rectangle(_selectionBoxPosition, _selectionBoxPosition + new Vec2(TileWidth, TileHeight)), Color.Black, 3f, false);

            if (_pages <= 1)
                return;

            float barRight = Position.x + width;
            float barLeft = barRight - _scrollBarWidth;
            float barTop = Position.y + _verticalGap;
            float barHeigth = height - (_verticalGap * 2f);

            Graphics.DrawRect(new Rectangle(new Vec2(barLeft, barTop), new Vec2(barRight, barTop + barHeigth)), Color.White, 3f); 

            float scrollBarHeight = barHeigth / _pages;
            float scrollBarTop = Position.y + _verticalGap + scrollBarHeight * _currentPage;

            Graphics.DrawRect(new Rectangle(new Vec2(barLeft, scrollBarTop), new Vec2(barRight, scrollBarTop + scrollBarHeight)), Color.Black, 3f);
        }

        public bool AddTile(UITile tile)
        {
            bool result = _tiles.Add(tile);

            if (result)
                ResetTileMap();

            return result;
        }

        public bool RemoveTile(UITile tile)
        {
            bool result = _tiles.Remove(tile);

            if (result)
                ResetTileMap();

            return result;
        }

        public void ClearTiles()
        {
            _tiles.Clear();
            ResetValues();
        }

        private void ResetTileMap()
        {
            _culomns = (int)Math.Floor(width / TileWidth);
            _rows = (int)Math.Floor(height / TileHeight);

            int tilesCount = _tiles.Count();

            _pages = (int)Math.Ceiling((float)tilesCount / (_rows * _culomns));

            _tileMap = new UITile[_culomns, _rows * _pages];

            _horizontalGap = (width - TileWidth * _culomns) / (_culomns + 1);
            _verticalGap = (height - TileHeight * _rows) / (_rows + 1);

            int tileIndex = 0;

            for (int page = 0; page < _pages; page++)
            {
                for (int row = 0; row < _rows; row++)
                {
                    for (int culomn = 0; culomn < _culomns; culomn++)
                    {
                        UITile tile = _tiles.ElementAt(tileIndex);

                        tile.Width = TileWidth;
                        tile.Height = TileHeight;

                        _tileMap[culomn, page * _rows + row] = tile;

                        tileIndex++;

                        if (tileIndex >= tilesCount)
                            goto exitLoop;
                    }
                }
            }

        exitLoop:;

            ResetValues();

            if (tilesCount > 0)
            {
                _selectedTile = _tiles.First();
                _bindBoxPosition = true;
            }
        }

        private UITile GetTile(int row, int culomn, int page)
        {
            return _tileMap[culomn, page * _rows + row];
        }

        private void ResetValues()
        {
            _currentPage = 0;
            _currentCulomn = 0;
            _currentRow = 0;
        }

        private void PlayTileChangeSound()
        {
            SFX.Play("textLetter", 0.7f);
        }
    }
}
