using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{
    [SerializeField]
    public static int MapSizeX = 64;

    [SerializeField]
    public static int MapSizeY = 64;

    [SerializeField]
    int MaxRoom = 10;

    [SerializeField]
    Tilemap tilemap_walls;

    [SerializeField]
    Tilemap floor_tilemap;

    [SerializeField]
    UnityEngine.Tilemaps.Tile wall;

    [SerializeField]
    UnityEngine.Tilemaps.Tile room;

    [SerializeField]
    UnityEngine.Tilemaps.Tile pass;

    [SerializeField]
    int enemyNumMin = 5;

    [SerializeField]
    int enemyNumMax = 10;

    private Tile[,] mapData;

    private List<GameObject> roomTilemapList;

    private Position playerSpawnPoint;
    private Vector3 playerSpawn;

    private Position nextFloorSpawnPoint;
    private Vector3 nextFloorSpawn;

    private List<Position> enemySpawnPointList;
    private List<Vector3> enemySpawnList;

    private MapGenerator generator = null;

    public Tile[,] GetMapData()
    {
        return this.mapData;
    }

    public void Create()
    {
        if (this.generator == null)
        {
            this.generator = new MapGenerator(MapSizeX, MapSizeY, MaxRoom);
        }

        this.MapDelete();
        this.GenerateMap();
        //this.Spawn();
    }

    private void Spawn()
    {
        this.SpawnPlayer();
        this.SpawnNextFloor();
        this.SpawnEnemies();
        this.SpawnItems();
    }

    private void GenerateMap()
    {
        this.generator.Generate();
        this.mapData = this.generator.GetMapData();

        /**/
        List<Range> roomList = this.generator.GetRoomData();
        List<Range> areaList = this.generator.GetAreaData();

        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject roomObject = new GameObject("room" + i);
            roomObject.tag = "Room";
            roomObject.transform.parent = tilemap_walls.transform.parent;
            var room_tilemap = roomObject.AddComponent<UnityEngine.Tilemaps.Tilemap>();
            roomObject.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();
            var collider = roomObject.AddComponent<UnityEngine.Tilemaps.TilemapCollider2D>();
            collider.isTrigger = true;

            Range areaRange = areaList[i];
            
            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (this.mapData[y, x].GetRoomNumber() == i)
                    {
                        room_tilemap.SetTile(new Vector3Int(x, y, 0), room);
                    }
                }
            }
            roomTilemapList.Add(roomObject);
        }
        /**/

        for (int y = 0; y < MapSizeY; y++)
        {
            for (int x = 0; x < MapSizeX; x++)
            {
                if (this.mapData[y, x].GetType() != TileType.None)
                {
                    switch (this.mapData[y, x].GetType())
                    {
                        case TileType.Wall:
                            tilemap_walls.SetTile(new Vector3Int(x, y, 0), wall);
                            break;
                        /**/
                        case TileType.Room:
                            //floor_tilemap.SetTile(new Vector3Int(x, y, 0), room);
                            break;
                        /**/
                        case TileType.Pass:
                            floor_tilemap.SetTile(new Vector3Int(x, y, 0), pass);
                            break;
                    }
                }
                CreateCircumscribedWall(x, y);
            }
        }
    }

    private void CreateCircumscribedWall(int x, int y)
    {
        if (this.mapData[0, x].GetType() != TileType.None)
        {
            CreateWall(x, -1);
        }
        if (this.mapData[y, 0].GetType() != TileType.None)
        {
            CreateWall(-1, y);
        }
        if (this.mapData[MapSizeY - 1, x].GetType() != TileType.None)
        {
            CreateWall(x, MapSizeY);
        }
        if (this.mapData[y, MapSizeX - 1].GetType() != TileType.None)
        {
            CreateWall(MapSizeX, y);
        }
    }

    private void CreateWall(int x, int y)
    {
        tilemap_walls.SetTile(new Vector3Int(x, y, 0), wall);
    }

    private void MapDelete()
    {
        if (roomTilemapList == null)
        {
            roomTilemapList = new List<GameObject>();
            roomTilemapList.Clear();
        }
        tilemap_walls.ClearAllTiles();
        floor_tilemap.ClearAllTiles();

        foreach(GameObject tilemap in roomTilemapList)
        {
            tilemap.GetComponent<UnityEngine.Tilemaps.Tilemap>().ClearAllTiles();
            Destroy(tilemap);
        }
        roomTilemapList.Clear();
    }

    private void SpawnPlayer()
    {
        Position spawn;
        do
        {
            spawn = new Position(RogueUtils.GetRandomInt(0, MapSizeX - 1), RogueUtils.GetRandomInt(0, MapSizeY - 1));
        } while (this.mapData[spawn.Y, spawn.X].GetType() != TileType.Room);

        this.playerSpawnPoint = spawn;
        this.playerSpawn = new Vector3(spawn.X + 0.5f, spawn.Y + 0.5f, 0);
    }

    public Vector3 GetPlayerSpawnPoint()
    {
        return this.playerSpawn;
    }

    private void SpawnNextFloor()
    {
        Position spawn;
        do
        {
            spawn = new Position(RogueUtils.GetRandomInt(0, MapSizeX - 1), RogueUtils.GetRandomInt(0, MapSizeY - 1));
        } while ((this.mapData[spawn.Y, spawn.X].GetType() != TileType.Room) || (spawn == this.playerSpawnPoint));

        this.nextFloorSpawnPoint = spawn;
        this.nextFloorSpawn = new Vector3(spawn.X + 0.5f, spawn.Y + 0.5f, 0);
    }

    public Vector3 GetNextFloorSpawnPoint()
    {
        return this.nextFloorSpawn;
    }

    private void SpawnEnemies()
    {
        if (enemySpawnList == null)
        {
            enemySpawnList = new List<Vector3>();
        }
        if (enemySpawnPointList == null)
        {
            enemySpawnPointList = new List<Position>();
        }
        enemySpawnList.Clear();
        enemySpawnPointList.Clear();

        int num = RogueUtils.GetRandomInt(enemyNumMin, enemyNumMax);
        for (int i = 0; i < num; i++)
        {
            Position spawn;
            while (true)
            {
                spawn = new Position(RogueUtils.GetRandomInt(0, MapSizeX - 1), RogueUtils.GetRandomInt(0, MapSizeY - 1));
                if (!enemySpawnPointList.Contains(spawn) &&
                    (!spawn.Equals(this.playerSpawnPoint)) &&
                    ((this.mapData[spawn.Y, spawn.X].GetType() == TileType.Room) ||
                    (this.mapData[spawn.Y, spawn.X].GetType() == TileType.Pass)))
                {
                    enemySpawnPointList.Add(spawn);
                    enemySpawnList.Add(new Vector3(spawn.X + 0.5f, spawn.Y + 0.5f, 0));
                    break;
                }
            }

        }
    }

    public List<Vector3> GetEnemySpawnPointList()
    {
        return enemySpawnList;
    }

    private void SpawnItems()
    {

    }
}