using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _grassTile, _mountainTile;

    [SerializeField] private Transform _camera;
    [SerializeField] private bool _mirrorField;
    [SerializeField] private int _minWalkableTiles;
    [SerializeField] private float _grassLevel;
    [SerializeField] private float _noiseScale;
    [SerializeField] private float _fallOffScale;

    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<Vector2, Tile> _playableTiles;
    public int Width => _width;
    public int Height => _height;

    void Awake() {
        Instance = this;
    }
    
    public void GenerateGrid() {
        Dictionary<Vector2, Tile> walkableTiles = new Dictionary<Vector2, Tile>();
        do
        {
            _tiles = new Dictionary<Vector2, Tile>();

            //Tile Generation
            (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    //Noise and FallOff maps obtained from @IndividualKex on Youtube
                    float noiseValue = Mathf.PerlinNoise(x * _noiseScale + xOffset, y * _noiseScale + yOffset);
                    float xv = x / (float)_width * 2 - 1;
                    float yv = y / (float)_height * 2 - 1;
                    float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                    float fallOffValue = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
                    noiseValue -= fallOffValue * _fallOffScale;

                    //var randomTile = Random.Range(0, 6) == 3 ? _mountainTile : _grassTile; //old
                    var newTile = noiseValue < _grassLevel ? _grassTile : _mountainTile;  //new

                    var spawnedTile = Instantiate(newTile, new Vector3(x, y), Quaternion.identity);
                    spawnedTile.name = $"Tile {x} {y}";
                    spawnedTile.Init(x, y);
                    _tiles[new Vector2(x, y)] = spawnedTile;
                    if(newTile == _grassTile)
                    {
                        walkableTiles[new Vector2(x, y)] = spawnedTile;
                    }
                    if (_mirrorField)
                    {
                        var mirroredX = _width - x - 1;
                        var mirroredTile = Instantiate(newTile, new Vector3(mirroredX, y), Quaternion.identity);
                        mirroredTile.name = $"Tile {mirroredX} {y}";
                        mirroredTile.Init(mirroredX, y);
                        //mirroredTile.GetComponent<SpriteRenderer>().color = UnityEngine.Color.red; //For Debugging
                        _tiles[new Vector2(mirroredX, y)] = mirroredTile;
                        if (newTile == _grassTile)
                        {
                            walkableTiles[new Vector2(mirroredX, y)] = mirroredTile;
                        }
                        if (x >= _width / 2 - 1 && _width % 2 == 0)
                        {
                            break;
                        }
                    }
                }
            }
        } while (checkMapValidity(walkableTiles));
        
        _camera.transform.position = new Vector3((float) _width / 2 - 0.5f, (float) _height / 2 - 0.5f, -10);        
        GameManager.Instance.ChangeState(Random.value >= 0.5 ? GameState.SpawnPlayerBuilding : GameState.SpawnEnemyBuilding);
    }

    
    private bool checkMapValidity(Dictionary<Vector2, Tile> walkableTiles)
    {
        //Regenerate map if not enough playable tiles found
        if (walkableTiles.Count < _minWalkableTiles) return false;

        //grab a random tile to start finding playable area
        _playableTiles = new Dictionary<Vector2, Tile>();
        var startTile = walkableTiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Key;

        if (doRecursiveFindTiles(startTile, walkableTiles) > _minWalkableTiles)
        {
            return true; //we have enough tiles for playable area
        }
        if(walkableTiles.Count > _minWalkableTiles)
        {
            return checkMapValidity(walkableTiles); //this will check other segmented spaces that might still contain playable areas
        }
        
        return false; //this map does not have a large enough play area, so we will need regenerate
    }

    private int doRecursiveFindTiles(Vector2 tile, Dictionary<Vector2, Tile> walkableTiles)
    {
        if (tile == null)
        {
            return 0;
        }
        //current tile is valid, so remove from walkable list and add to playable list
        walkableTiles.Remove(tile);
        _playableTiles.Add(tile, getTileAtPos(tile, _playableTiles));

        Tile north = getTileNorth(tile, walkableTiles);
        Tile south = getTileSouth(tile, walkableTiles);
        Tile east = getTileEast(tile, walkableTiles);
        Tile west = getTileWest(tile, walkableTiles);

        return 1 
            + doRecursiveFindTiles(north.transform.position, walkableTiles)
            + doRecursiveFindTiles(south.transform.position, walkableTiles)
            + doRecursiveFindTiles(east.transform.position, walkableTiles)
            + doRecursiveFindTiles(west.transform.position, walkableTiles);
    }

    //rework
    public Tile GetPlayerBuildingSpawnTile()
    {
        return _playableTiles.Where(t => t.Key.x < _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    //rework
    public Tile GetEnemyBuildingSpawnTile()
    {
        return _playableTiles.Where(t => t.Key.x > _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    //rework
    public Tile GetPlayerSpawnTile()
    {
        return _playableTiles.Where(t => t.Key.x < _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    //rework
    public Tile GetEnemySpawnTile()
    {
        return _playableTiles.Where(t => t.Key.x > _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }
    private Tile getTileAtPos(Vector2 pos, Dictionary<Vector2, Tile> tiles)
    {
        if (tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }
    public Tile GetTileAtPosition(Vector2 pos){   
        return getTileAtPos(pos, _tiles);
    }

    private Tile getTileNorth(Vector2 pos, Dictionary<Vector2, Tile> tiles)
    {
        if (tiles.TryGetValue(new Vector2(pos.x, pos.y + 1), out var tile))
        {
            return tile;            
        }
        return null;
    }

    private Tile getTileSouth(Vector2 pos, Dictionary<Vector2, Tile> tiles)
    {
        if (tiles.TryGetValue(new Vector2(pos.x, pos.y - 1), out var tile))
        {
            return tile;
        }
        return null;
    }

    private Tile getTileEast(Vector2 pos, Dictionary<Vector2, Tile> tiles)
    {
        if (tiles.TryGetValue(new Vector2(pos.x + 1, pos.y), out var tile))
        {
            return tile;
        }
        return null;
    }

    private Tile getTileWest(Vector2 pos, Dictionary<Vector2, Tile> tiles)
    {
        if (tiles.TryGetValue(new Vector2(pos.x - 1, pos.y), out var tile))
        {
            return tile;
        }
        return null;
    }

    public Tile GetTileNorthOfPosition(Vector2 pos)
    {
        return getTileNorth(pos, _tiles);
    }

    public Tile GetTileSouthOfPosition(Vector2 pos)
    {
        return getTileSouth(pos, _tiles);
    }

    public Tile GetTileEastOfPosition(Vector2 pos)
    {
        return getTileEast(pos, _tiles);
    }

    public Tile GetTileWestOfPosition(Vector2 pos)
    {
        return getTileWest(pos, _tiles);
    }
}
