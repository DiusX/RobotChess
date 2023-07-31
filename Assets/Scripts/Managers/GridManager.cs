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
    [SerializeField] private bool _isMirroredMap;
    [SerializeField] private int _minWalkableTiles;
    [SerializeField] private float _grassLevel;
    [SerializeField] private float _noiseScale;
    [SerializeField] private float _fallOffScale;

    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<Vector2, Tile> _walkableTiles;
    private Dictionary<Vector2, Tile> _playableTiles;
    public int Width => _width;
    public int Height => _height;

    void Awake() {
        Instance = this;
    }
    private int counter = 0;
    public void GenerateGrid() {
        _camera.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
        _tiles = new Dictionary<Vector2, Tile>();
        _walkableTiles = new Dictionary<Vector2, Tile>();
        _playableTiles = new Dictionary<Vector2, Tile>();

        do
        {
            //Debug.Log("_tiles count: " + _tiles.Count);
            foreach (var tileEntry in _tiles)
            {
                Destroy(tileEntry.Value.gameObject); //destroys the gameobject that the "tile" component has been assigned to, removing the tile from the scene as a result
            }
            _tiles.Clear();
            _walkableTiles.Clear();
            _playableTiles.Clear();

            if (counter > 3000)
            {
                break;
            }
            counter++;
            Debug.Log("RUNNING #" + counter);

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
                    if (newTile == _grassTile)
                    {
                        _walkableTiles[new Vector2(x, y)] = spawnedTile;
                    }
                    if (_isMirroredMap)
                    {
                        var mirroredX = _width - x - 1;
                        var mirroredTile = Instantiate(newTile, new Vector3(mirroredX, y), Quaternion.identity);
                        mirroredTile.name = $"Tile {mirroredX} {y}";
                        mirroredTile.Init(mirroredX, y);
                        //mirroredTile.GetComponent<SpriteRenderer>().color = UnityEngine.Color.yellow; //For Debugging
                        _tiles[new Vector2(mirroredX, y)] = mirroredTile;
                        if (newTile == _grassTile)
                        {
                            _walkableTiles[new Vector2(mirroredX, y)] = mirroredTile;
                        }
                        if (x >= _width / 2 - 1 && _width % 2 == 0)
                        {
                            break;
                        }
                    }
                }
            }
            expandMapPaths();

        } while (!checkMapValidity());

        Debug.Log("FINAL EXIT");
        //GameManager.Instance.ChangeState(Random.value >= 0.5 ? GameState.SpawnPlayerBuilding : GameState.SpawnEnemyBuilding);
    }

    private void expandMapPaths()
    {
        List<Vector2> walkables = new List<Vector2>();
        foreach (var tilePos in _walkableTiles.Keys) {
            walkables.Add(tilePos);
        }
        while (walkables.Count > 0)
        {
            Vector2 tileToExpand = walkables.First();
            walkables.Remove(tileToExpand);
            expandPath(tileToExpand, walkables);
        }
        //maybe separate mirroredTile breaking to end. (To account for randomness inbetween) - then only call breakTile on those
    }

    private pathDirection expandPath(Vector2 tilePos, List<Vector2> walkables)
    {
        Tile tileNorth = getTileNorth(tilePos, _walkableTiles);
        Tile tileSouth = getTileSouth(tilePos, _walkableTiles);
        Tile tileEast = getTileEast(tilePos, _walkableTiles);
        Tile tileWest = getTileWest(tilePos, _walkableTiles);

        if (tileNorth != null && tileSouth != null && tileEast == null && tileWest == null)
        { //This is a vertical pathway so we break horizontally
            Vector2 tilePosToBreak = Random.value >= 0.5 ? new Vector2(tilePos.x+1, tilePos.y) : new Vector2(tilePos.x-1, tilePos.y);
            Tile tileToBreak = getTileAtPos(tilePosToBreak, _tiles);
            breakTileOpen(tileToBreak);
            _walkableTiles.Add(tilePosToBreak, tileToBreak);
            walkables.Add(tilePosToBreak);
            if (_isMirroredMap)
            {
                Vector2 tilePosToBreakMirrored = new Vector2(_width - tilePosToBreak.x - 1, tilePosToBreak.y);
                Tile tileToBreakMirrored = getTileAtPos(tilePosToBreakMirrored, _tiles);
                breakTileOpen(tileToBreakMirrored);
                _walkableTiles.Add(tilePosToBreakMirrored, tileToBreakMirrored);
                walkables.Add(tilePosToBreakMirrored);
            }
            return pathDirection.vertical;
        }
        if(tileNorth == null && tileSouth == null && tileEast != null && tileWest != null)
        { //This is a horizontal pathway so we break vertically
            Vector2 tilePosToBreak = Random.value >= 0.5 ? new Vector2(tilePos.x, tilePos.y + 1) : new Vector2(tilePos.x, tilePos.y - 1);
            Tile tileToBreak = getTileAtPos(tilePosToBreak, _tiles);
            breakTileOpen(tileToBreak);
            _walkableTiles.Add(tilePosToBreak, tileToBreak);
            walkables.Add(tilePosToBreak);
            if (_isMirroredMap)
            {
                Vector2 tilePosToBreakMirrored = new Vector2(tilePosToBreak.x, _height - tilePosToBreak.y - 1);
                Tile tileToBreakMirrored = getTileAtPos(tilePosToBreakMirrored, _tiles);
                breakTileOpen(tileToBreakMirrored);
                _walkableTiles.Add(tilePosToBreakMirrored, tileToBreakMirrored);
                walkables.Add(tilePosToBreakMirrored);
            }
            return pathDirection.horizontal;
        }

        return pathDirection.none;
    }

    private void breakTileOpen(Tile tile)
    {
        tile.breakTileOpen(_grassTile.GetComponent<SpriteRenderer>().color); //TODO: rework by deleting mountain tile and replacing with newly created grass tile
    }

    private enum pathDirection {
        vertical = 0, horizontal = 1, none = 2
    }
    
    private bool checkMapValidity()
    {
        //Regenerate map if not enough playable tiles found
        if (_walkableTiles.Count < _minWalkableTiles) return false;

        _playableTiles.Clear();        

        //grab a random tile to start finding playable area
        var startTileEntry = _walkableTiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First();

        if (doRecursiveFindTiles(startTileEntry) > _minWalkableTiles)
        {
            Debug.Log("Completed Grid Generation with walkableTiles left:  " + _walkableTiles.Count);
            Debug.Log("Playable tiles found in last run:  " + _playableTiles.Count);
            return true; //we have enough tiles for playable area
        }
        if(_walkableTiles.Count >= _minWalkableTiles)
        {
            Debug.Log("Retrying Grid Generation with walkableTiles left:  " + _walkableTiles.Count);
            Debug.Log("Playable tiles found in last run:  " + _playableTiles.Count);
            foreach (var tile in _playableTiles.Values) {
                Debug.Log(tile.TileName);
                tile.gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.red; //For Debugging
            }
            return checkMapValidity(); //this will check other segmented spaces that might still contain playable areas
        }        
        Debug.Log("Failed Grid Generation with walkableTiles left:  *" + _walkableTiles.Count);
        Debug.Log("Playable tiles found in last run:  " + _playableTiles.Count);
        return false; //this map does not have a large enough play area, so we will need regenerate
    }

    private int doRecursiveFindTiles(KeyValuePair<Vector2, Tile> tileEntry)
    {
        //Debug.Log("GETTING TILE:  " + tile.ToString());
       
        _walkableTiles.Remove(tileEntry.Key);
        tileEntry.Value.GetComponent<SpriteRenderer>().color = UnityEngine.Color.blue; //For Debugging
        if (_playableTiles.Contains(tileEntry))
        {
            Debug.Log("DUPLICATION BUG");
            return 0;
        }
        _playableTiles.Add(tileEntry.Key, tileEntry.Value);
        
        int subtreeCount = 0;
        Tile north = getTileNorth(tileEntry.Key, _walkableTiles);        
        if (north != null)
        {
            /*Debug.Log("GETTING TILE:  " + tile.ToString());
            Debug.Log("with north tile:  " + north.ToString());*/
            KeyValuePair<Vector2, Tile> northEntry = new KeyValuePair<Vector2, Tile>(north.transform.position, north);
            subtreeCount += doRecursiveFindTiles(northEntry);
        }
        Tile south = getTileSouth(tileEntry.Key, _walkableTiles);        
        if (south != null)
        {
            /* Debug.Log("GETTING TILE:  " + tile.ToString());
             Debug.Log("with south tile:  " + south.ToString());*/
            KeyValuePair<Vector2, Tile> southEntry = new KeyValuePair<Vector2, Tile>(south.transform.position, south);
            subtreeCount += doRecursiveFindTiles(southEntry);
        }
        Tile east = getTileEast(tileEntry.Key, _walkableTiles);
        if (east != null)
        {
            /*Debug.Log("GETTING TILE:  " + tile.ToString());
            Debug.Log("with east tile:  " + east.ToString());*/
            KeyValuePair<Vector2, Tile> eastEntry = new KeyValuePair<Vector2, Tile>(east.transform.position, east);
            subtreeCount += doRecursiveFindTiles(eastEntry);
        }
        Tile west = getTileWest(tileEntry.Key, _walkableTiles);     
        if (west != null)
        {
            /*Debug.Log("GETTING TILE:  " + tile.ToString());
            Debug.Log("with west tile:  " + west.ToString());*/
            KeyValuePair<Vector2, Tile> westEntry = new KeyValuePair<Vector2, Tile>(west.transform.position, west);
            subtreeCount += doRecursiveFindTiles(westEntry);
        }

        return subtreeCount + 1;
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
