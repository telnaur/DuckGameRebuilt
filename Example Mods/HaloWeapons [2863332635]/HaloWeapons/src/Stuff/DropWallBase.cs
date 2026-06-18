using System;
using System.Collections.Generic;

namespace DuckGame.HaloWeapons
{
    public class DropWallBase : SyncedPositionBlock
    {
        private readonly List<DropWallTile> _tiles;
        private readonly float _tileOffsetX = 1f;
        private readonly float _maxTileHeight = 15f;
        private readonly int _tilesCount = 3;

        [Binding] private BitBuffer _tilesBuffer = new BitBuffer();
        [Binding] private bool _unfolding;

        public DropWallBase(float x, float y) : base(x, y)
        {
            graphic = Resources.LoadSprite("dropWallBase.png");
            graphic.flipH = Direction < 0;
            collisionSize = new Vec2(10f, 6f);

            _tiles = new List<DropWallTile>(_tilesCount);
        }

        public override void Initialize()
        {
            if (isServerForObject)
            {
                for (int i = 0; i < _tilesCount; i++)
                {
                    var tile = new DropWallTile(x + _tileOffsetX, y);

                    tile.y -= (tile.height - 1f) * (i + 1f);
                    tile.Direction = Direction;

                    Level.Add(tile);
                    _tiles.Add(tile);
                }

                _unfolding = true;
            }
        }

        public override void Update()
        {
            base.Update();

            if (isServerForObject)
            {
                if (Network.isActive)
                {
                    _tilesBuffer.Clear();

                    foreach (DropWallTile tile in _tiles)
                    {
                        NetworkConnection localConnection = DuckNetwork.localConnection;

                        if (tile is not null && !tile.removeFromLevel)
                        {
                            if (tile.connection != localConnection)
                                SuperFondle(tile, localConnection);

                            _tilesBuffer.WriteObject(tile);
                        }
                    }
                }

                foreach (DropWallTile tile in _tiles)
                    if (tile is not null && tile.BreakEventIsEmpty)
                        tile.Break += OnTileBroke;

                if (_unfolding)
                {
                    if (_tiles[0].Height >= _maxTileHeight)
                    {
                        foreach (DropWallTile tile in _tiles)
                            tile.Active = true;

                        _unfolding = false;
                    }
                    else
                    {
                        for (int i = 0; i < _tiles.Count; i++)
                        {
                            DropWallTile tile = _tiles[i];

                            tile.Height += 1f;
                            tile.y -= 1f * i + 1f;
                        }
                    }
                }
            }
            else
            {
                _tiles.Clear();

                while (true)
                {
                    try
                    {
                        _tiles.Add(_tilesBuffer.ReadThing(typeof(DropWallTile)) as DropWallTile);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        public override void Draw()
        {
            Graphics.Draw(graphic, x - 2f, y);
        }

        private void OnTileBroke(object sender, EventArgs eventArgs)
        {
            DevConsole.Log("AMOGUS");

            int index = _tiles.IndexOf(sender as DropWallTile) + 1;

            for (int i = index; i < _tiles.Count; i++)
                Level.Remove(_tiles[i]);
        }
    }
}
