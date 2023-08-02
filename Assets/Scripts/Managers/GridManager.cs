using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// This Singleton class manages the generation of a valid map that could be played on.
    /// </summary>
    /// TODO: Adjust class to be server code.
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

    /// <summary>
    /// <para>Generates a grid of tiles that satifies predefined conditions. </para>
    /// First, the tiles are generated with a specific 'level' value based on Perlin/Simplex noise and a Falloff map. This determines if the tile will be grass or mountain. <br />
    /// Second, the mountain tiles are expanded to make the map more playable. <br />
    /// Lastly, we check whether the map at this point is playable and satisfies the predefined conditions. Restart this generation process if not.
    /// </summary>
    /// <exception cref="System.Exception">Throws System.Exception when the width and height make up an area size that is less than the assigned minimum Walkable tiles</exception>
    public void GenerateGrid() {
        _camera.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
        _tiles = new Dictionary<Vector2, Tile>();
        _walkableTiles = new Dictionary<Vector2, Tile>();
        _playableTiles = new Dictionary<Vector2, Tile>();

        if(_width * _height < _minWalkableTiles)
        {
            throw new System.Exception("Play Area size does not meet requirements. Adjust width and height");
        }

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

            Debug.Log("RUNNING #" + counter);
            if (counter > 1)
            {
                break;
            }
            counter++;
            

            //Tile Generation
            (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    //Functions for Noise and FallOff maps obtained from @IndividualKex on Youtube
                    float noiseValue = Mathf.PerlinNoise(x * _noiseScale + xOffset, y * _noiseScale + yOffset);
                    float xv = x / (float)_width * 2 - 1;
                    float yv = y / (float)_height * 2 - 1;
                    float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                    float fallOffValue = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
                    noiseValue -= fallOffValue * _fallOffScale;
                    
                    var newTile = noiseValue < _grassLevel ? _grassTile : _mountainTile;

                    var spawnedTile = Instantiate(newTile, new Vector3(x, y), Quaternion.identity);
                    spawnedTile.name = $"Tile {x} {y}";
                    spawnedTile.Init(x, y);
                    _tiles[new Vector2(x, y)] = spawnedTile;
                    if (newTile == _grassTile)
                    {
                        _walkableTiles[new Vector2(x, y)] = spawnedTile;
                    }
                    //if map is to be mirrored, duplicate tiles to other side of map and stop generation at certain point depending on the width of the map.
                    if (_isMirroredMap)
                    {
                        var mirroredX = _width - x - 1;
                        if (_width % 2 == 1 && x >= mirroredX)
                        {
                            break;
                        }                       
                        var mirroredTile = Instantiate(newTile, new Vector3(mirroredX, y), Quaternion.identity);
                        mirroredTile.name = $"Tile {mirroredX} {y}";
                        mirroredTile.Init(mirroredX, y);
                        //mirroredTile.GetComponent<SpriteRenderer>().color = UnityEngine.Color.yellow; //For Debugging
                        _tiles[new Vector2(mirroredX, y)] = mirroredTile;
                        if (newTile == _grassTile)
                        {
                            _walkableTiles[new Vector2(mirroredX, y)] = mirroredTile;
                        }
                        if (_width % 2 == 0 && x >= mirroredX - 1)
                        {
                            break;
                        }
                    }
                }
            }
            counterBreak = 0;
            expandMapPaths();
            Debug.Log("BREAK COUNTER:  " + counterBreak);
        } while (!checkMapValidity());

        Debug.Log("FINAL EXIT");
        //GameManager.Instance.ChangeState(Random.value >= 0.5 ? GameState.SpawnPlayerBuilding : GameState.SpawnEnemyBuilding);
    }

    /// <summary>
    /// Expand on mountains that are next to a grass tile that forms part of a single lane. Mirror the expansion accordingly if required.
    /// </summary>
    private void expandMapPaths()
    {
        List<Vector2> walkables = new List<Vector2>();
        List<Vector2> walkablesMirrored = new List<Vector2>();
        if (_isMirroredMap)
        {
            foreach (var tilePos in _walkableTiles.Keys.Where(t => t.x < _width / 2))
            {
                walkables.Add(tilePos);
            }
            if (_width%2 == 1)
            {
                //do additional run on middle
                foreach (var tilePos in _walkableTiles.Keys.Where(t => t.x == _width / 2))
                {
                    walkables.Add(tilePos);
                }
            }
        }
        else
        {
            foreach (var tilePos in _walkableTiles.Keys) { 
                walkables.Add(tilePos);
            }
        }
        while (walkables.Count > 0)
        {
            Vector2 tileToExpand = walkables.First();
            walkables.Remove(tileToExpand);
            expandPath(tileToExpand, walkables, walkablesMirrored);
        }
        while (walkablesMirrored.Count > 0)
        {
            Vector2 tilePosToBreakMirrored = walkablesMirrored.First();
            walkablesMirrored.Remove(tilePosToBreakMirrored);
            Tile tileToBreakMirrored = getTileAtPos(tilePosToBreakMirrored, _tiles);
            breakTileOpen(tilePosToBreakMirrored, tileToBreakMirrored);
        }
    }

    private void expandPath(Vector2 tilePos, List<Vector2> walkables, List<Vector2> walkablesMirrored)
    {
        Tile tileNorth = getTileNorth(tilePos, _walkableTiles);
        Tile tileSouth = getTileSouth(tilePos, _walkableTiles);
        Tile tileEast = getTileEast(tilePos, _walkableTiles);
        Tile tileWest = getTileWest(tilePos, _walkableTiles);

        //check for a vertical pathway that we should break horizontally
        if (tileNorth != null && tileSouth != null && tileEast == null && tileWest == null)
        {
            if (_isMirroredMap)
            {
                if (_width % 2 == 0 && tilePos.x + 1 == _width / 2)
                {
                    //no need to expand since mirrored tile will be adjacent and also open
                    return;
                }
                if (tilePos.x > _width / 2)
                {
                    Debug.Log("MIRRORED AREA EXPAND BUG");
                    return;
                }
                Vector2 tilePosToBreak;
                if (_width % 2 == 1 && tilePos.x + 1 == _width / 2)
                {
                    //forced left, since this is the column in the centre
                    tilePosToBreak = new Vector2(tilePos.x - 1, tilePos.y);
                }
                else if (tilePos.x == 0)
                {
                    //forced right, since left is off of map
                    tilePosToBreak = new Vector2(tilePos.x + 1, tilePos.y);
                }
                else
                {
                    tilePosToBreak = Random.value >= 0.5 ? (new Vector2(tilePos.x + 1, tilePos.y)) : (new Vector2(tilePos.x - 1, tilePos.y));
                }

                Tile tileToBreak = getTileAtPos(tilePosToBreak, _tiles);
                breakTileOpen(tilePosToBreak, tileToBreak);
                walkables.Add(tilePosToBreak); //check if tile brings forth further expansion

                Vector2 tilePosToBreakMirrored = new Vector2(_width - tilePosToBreak.x - 1, tilePosToBreak.y);
                walkablesMirrored.Add(tilePosToBreakMirrored);
            }
            else
            {
                Vector2 tilePosToBreak;
                if (tilePos.x == 0)
                {
                    //forced right, since left is off of map
                    tilePosToBreak = new Vector2(tilePos.x + 1, tilePos.y);
                }
                else if(tilePos.x == _width - 1)
                {
                    //forced left, since right is off of map
                    tilePosToBreak = new Vector2(tilePos.x - 1, tilePos.y);
                }
                else
                {
                    tilePosToBreak = Random.value >= 0.5 ? new Vector2(tilePos.x + 1, tilePos.y) : new Vector2(tilePos.x - 1, tilePos.y);
                }
                                
                Tile tileToBreak = getTileAtPos(tilePosToBreak, _tiles);
                breakTileOpen(tilePosToBreak, tileToBreak);
                walkables.Add(tilePosToBreak); //check if tile brings forth further expansion
            }
        }
        //check for a horizontal pathway that we should break vertically
        else if (tileNorth == null && tileSouth == null && tileEast != null && tileWest != null)
        {
            Vector2 tilePosToBreak;
            if (tilePos.y == 0)
            {
                //forced up, since down is off of map
                tilePosToBreak = new Vector2(tilePos.x, tilePos.y + 1);
            }
            else if (tilePos.y == _height - 1)
            {
                //forced down, since top is off of map
                tilePosToBreak = new Vector2(tilePos.x, tilePos.y - 1);
            }
            else
            {
                tilePosToBreak = Random.value >= 0.5 ? new Vector2(tilePos.x, tilePos.y + 1) : new Vector2(tilePos.x, tilePos.y - 1);
            }
            
            Tile tileToBreak = getTileAtPos(tilePosToBreak, _tiles);
            breakTileOpen(tilePosToBreak, tileToBreak);
            walkables.Add(tilePosToBreak); //check if tile brings forth further expansion
            
            if (_isMirroredMap)
            {
                Vector2 tilePosToBreakMirrored = new Vector2(_width - tilePosToBreak.x - 1, tilePosToBreak.y);
                walkablesMirrored.Add(tilePosToBreakMirrored);
            }
        }
    }

    private int counterBreak = 0;

    private void breakTileOpen(Vector2 pos, Tile tile)
    {
        var spawnedTile = Instantiate(_grassTile, pos, Quaternion.identity);
        spawnedTile.name = $"Tile {pos.x} {pos.y}";
        spawnedTile.Init((int)pos.x, (int)pos.y);
        spawnedTile.gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.grey; //For Debugging
        _tiles[pos] = spawnedTile;
        _walkableTiles[pos] = spawnedTile;
        Destroy(tile.gameObject);
        counterBreak++;
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
                //tile.gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.red; //For Debugging
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
        //tileEntry.Value.GetComponent<SpriteRenderer>().color = UnityEngine.Color.blue; //For Debugging
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
