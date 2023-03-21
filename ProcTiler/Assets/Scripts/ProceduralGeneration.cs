// This code uses a third-party Tags package from neon-age. (https://github.com/neon-age/Tags)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float generationRadius;
    [SerializeField] private float tileSize;

    [SerializeField] private GameObject[] forwardTiles;
    [SerializeField] private int[] forwardProbability;
    private GameObject[] forwardTilesWithProbability;

    [SerializeField] private GameObject[] backTiles;
    [SerializeField] private int[] backProbability;
    private GameObject[] backTilesWithProbability;

    [SerializeField] private GameObject[] leftTiles;
    [SerializeField] private int[] leftProbability;
    private GameObject[] leftTilesWithProbability;

    [SerializeField] private GameObject[] rightTiles;
    [SerializeField] private int[] rightProbability;
    private GameObject[] rightTilesWithProbability;

    [SerializeField] private GameObject allDirection;

    [SerializeField] private Tag toForward;
    [SerializeField] private Tag toBack;
    [SerializeField] private Tag toLeft;
    [SerializeField] private Tag toRight;

    [SerializeField] private Tag Wall;

    private List<GameObject> lastRendered = new List<GameObject>();
    private List<GameObject> toRender = new List<GameObject>();

    private ObjectPool objectPool;

    #region Editor stuff
    private void OnValidate()
    {
        SyncProbabilityToTiles(forwardTiles, ref forwardProbability, ref forwardTilesWithProbability);
        SyncProbabilityToTiles(backTiles, ref backProbability, ref backTilesWithProbability);
        SyncProbabilityToTiles(leftTiles, ref leftProbability, ref leftTilesWithProbability);
        SyncProbabilityToTiles(rightTiles, ref rightProbability, ref rightTilesWithProbability);
    }

    void SyncProbabilityToTiles (GameObject[] tiles, ref int[] probabilities, ref GameObject[] tilesWithProbability)
    {
        if (probabilities == null || probabilities.Length != tiles.Length)
        {
            probabilities = new int[tiles.Length];
        }
    }
    #endregion
    
    private void Awake() {
        ProbabilityAdder(forwardTiles, forwardProbability, ref forwardTilesWithProbability);
        ProbabilityAdder(backTiles, backProbability, ref backTilesWithProbability);
        ProbabilityAdder(leftTiles, leftProbability, ref leftTilesWithProbability);
        ProbabilityAdder(rightTiles, rightProbability, ref rightTilesWithProbability);
    }
    
    private void Start()
    {
        objectPool = gameObject.AddComponent<ObjectPool>();
        lastRendered.Add(Instantiate(forwardTiles[0], Vector3.zero, Quaternion.identity));
    }

    private void Update()
    {
        Generate();
    }

    private void Generate()
    {
        Vector3 playerPosition = playerTransform.position;
        DestroyTiles();

        List<GameObject> newTiles = new List<GameObject>();
        for (int i = 0; i < lastRendered.Count; i++)
        {
            GameObject tile = lastRendered[i];
            Vector3 tilePosition = tile.transform.position;

            if (tile.HasTag(toForward))
            {
                Vector3 newPosition = tilePosition + new Vector3(0, 0, tileSize);
                if (!IsOccupied(newPosition) && Vector3.Distance(newPosition, playerPosition) <= generationRadius)
                {
                    GameObject newTile;
            
                    if (tile.HasOnlyTag(toForward))
                    {
                        newTile = objectPool.GetObject(forwardTilesWithProbability[Random.Range(0, forwardTilesWithProbability.Length)], newPosition, Quaternion.identity);
                    }
                    else
                    {
                        newTile = objectPool.GetObject(forwardTiles[0], newPosition, Quaternion.identity);
                    }
                    newTiles.Add(newTile);
                }
            }

            if (tile.HasTag(toBack))
            {
                Vector3 newPosition = tilePosition + new Vector3(0, 0, -tileSize);
                if (!IsOccupied(newPosition) && Vector3.Distance(newPosition, playerPosition) <= generationRadius)
                {
                    GameObject newTile;

                    if (tile.HasOnlyTag(toBack))
                    {
                        newTile = objectPool.GetObject(backTilesWithProbability[Random.Range(0, backTilesWithProbability.Length)], newPosition, Quaternion.identity);
                    }
                    else
                    {
                        newTile = objectPool.GetObject(backTiles[0], newPosition, Quaternion.identity);
                    }
                    newTiles.Add(newTile);
                }
            }

            if (tile.HasTag(toLeft))
            {
                Vector3 newPosition = tilePosition + new Vector3(-tileSize, 0, 0);
                if (!IsOccupied(newPosition) && Vector3.Distance(newPosition, playerPosition) <= generationRadius)
                {
                    GameObject newTile;

                    if (tile.HasOnlyTag(toLeft))
                    {
                        newTile = objectPool.GetObject(leftTilesWithProbability[Random.Range(0, leftTilesWithProbability.Length)], newPosition, Quaternion.identity);
                    }
                    else
                    {
                        newTile = objectPool.GetObject(leftTiles[0], newPosition, Quaternion.identity);
                    }
                    newTiles.Add(newTile);
                }
            }

            if (tile.HasTag(toRight))
            {
                Vector3 newPosition = tilePosition + new Vector3(tileSize, 0, 0);
                if (!IsOccupied(newPosition) && Vector3.Distance(newPosition, playerPosition) <= generationRadius)
                {
                    GameObject newTile;

                    if (tile.HasOnlyTag(toRight))
                    {
                        newTile = objectPool.GetObject(rightTilesWithProbability[Random.Range(0, rightTilesWithProbability.Length)], newPosition, Quaternion.identity);
                    }
                    
                    else
                    {
                        newTile = objectPool.GetObject(rightTiles[0], newPosition, Quaternion.identity);
                    }
                    newTiles.Add(newTile);
                }
            }
        }

        lastRendered.AddRange(newTiles);
        
    }

    private void DestroyTiles()
    {
        List<GameObject> tilesToDestroy = new List<GameObject>();
        Vector3 playerPosition = playerTransform.position;

        for (int i = 0; i < lastRendered.Count; i++)
        {
            GameObject tile = lastRendered[i];

            float distanceToPlayer = Vector3.Distance(tile.transform.position, playerPosition);
            if (distanceToPlayer > generationRadius)
            {
                tilesToDestroy.Add(tile);
            }
        }

        for (int i = 0; i < tilesToDestroy.Count; i++)
        {
            GameObject tile = tilesToDestroy[i];
            lastRendered.Remove(tile);
            objectPool.ReturnObject(tile);
        }
    }

    bool IsOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, new Vector3(tileSize /3, tileSize /3, tileSize /3));   
        return colliders.Length > 0;
    }

    private void ProbabilityAdder(GameObject[] tiles, int[] probabilities, ref GameObject[] result)
    {
        result = new GameObject[probabilities.Sum()];
        int index = 0;
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < probabilities[i]; j++)
            {
                result[index++] = tiles[i];
            }
        }
    }
}

