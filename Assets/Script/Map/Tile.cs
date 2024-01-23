using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None = (-1),    // なし
    Wall,   // 壁のタイル
    Room,   // 部屋のタイル
    Pass    // 通路のタイル
}

public class Tile
{
    private TileType type;
    private Position position;
    private int roomNumber = -1;

    public Tile(TileType type, Position position)
    {
        this.type = type;
        this.position = new Position(position.X, position.Y);
    }

    public void SetType(TileType type)
    {
        this.type = type;
    }

    public TileType GetType()
    {
        return this.type;
    }

    public void SetRoomNumber(int roomNumber)
    {
        this.roomNumber = roomNumber;
    }

    public int GetRoomNumber()
    {
        return this.roomNumber;
    }

    public void SetPosition(int x, int y)
    {
        this.position = new Position(x, y);
    }

    public void SetPosition(Position pos)
    {
        this.position = new Position(pos.X, pos.Y);
    }

    public Position GetPosition()
    {
        return this.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}