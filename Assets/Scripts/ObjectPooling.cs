using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private int poolSize = 100;

    [SerializeField] private List<Tile> objectPoolTiles = new List<Tile>();
    public List<Tile> ObjectPoolTiles => objectPoolTiles;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Tile obj = Instantiate(tilePrefab);
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(gameObject.transform);
            objectPoolTiles.Add(obj);
        }
    }

    public Tile GetObjectFromPool()
    {
        foreach (var tile in objectPoolTiles)
        {
            if (!tile.gameObject.activeInHierarchy)
            {
                tile.gameObject.SetActive(true);
                return tile;
            }
        }
        return null;
    }

    public void ReturnObjectToPool(Tile tile)
    {
        tile.gameObject.SetActive(false);
    }
}
