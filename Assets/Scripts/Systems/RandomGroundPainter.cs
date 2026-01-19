using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomGroundPainter : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] grassTiles;

    public int width = 50;
    public int height = 50;

    [ContextMenu("Generate Ground")]
    void Generate()
    {
        tilemap.ClearAllTiles();

        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int y = -height / 2; y < height / 2; y++)
            {
                var tile = grassTiles[Random.Range(0, grassTiles.Length)];
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
