using UnityEngine.Tilemaps;
using UnityEngine;

namespace PuzzleRoom.Data
{
    [System.Serializable]
    public class TilemapSectionData
    {
        public Vector3Int BottomLeftPosition;

        public int Size = 4;
        public TileBase[,] TileBases;

        public int StartX;
        public int StartY;

        public GameObject sectionGameObject;

        public TilemapSectionData(int startX, int startY, int size, Tilemap tilemap)
        {
            StartX = startX;
            StartY = startY;
            Size = size;
            TileBases = new TileBase[Size, Size];

            BottomLeftPosition = new Vector3Int(StartX, StartY, 0);
            TileBases = new TileBase[size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(StartX + x, StartY + y, 0);
                    TileBases[x, y] = tilemap.GetTile(tilePosition);
                }
            }
        }

        public void ClearTiles(Tilemap tilemap)
        {
            for (int x = 0; x < TileBases.GetLength(0); x++)
            {
                for (int y = 0; y < TileBases.GetLength(1); y++)
                {
                    Vector3Int tilePosition = new Vector3Int(BottomLeftPosition.x + x, BottomLeftPosition.y + y, 0);
                    tilemap.SetTile(tilePosition, null);
                }
            }
        }
    }

}