using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] private GameObject player;
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


    private List<GameObject> lastRendered = new List<GameObject>();
    private List<GameObject> toRender = new List<GameObject>();
    private bool generated = false;

    [SerializeField] private Tag toForward;
    [SerializeField] private Tag toBack;
    [SerializeField] private Tag toLeft;
    [SerializeField] private Tag toRight;

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

        ProbabilityAdder(tiles, probabilities, ref tilesWithProbability);
    }

    #endregion
    
    
    void Start()
    {
        lastRendered.Add(Instantiate(forwardTiles[0], Vector3.zero, Quaternion.identity));
    }

    void Update()
    {
        if (generated == false)
        {
            StartCoroutine(Generate());
            generated = true;
        }
    }

    IEnumerator Generate()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < lastRendered.Count; i++)
        {
            if (lastRendered[i].HasTag(toForward))
            {
                Vector3 newPosition = lastRendered[i].transform.position + new Vector3(0, 0, tileSize);
                if (!IsOccupied(newPosition))
                {
                    toRender.Add(Instantiate(forwardTilesWithProbability[Random.Range(0, forwardTilesWithProbability.Length)], newPosition, Quaternion.identity));
                }
            }
            if (lastRendered[i].HasTag(toBack))
            {
                Vector3 newPosition = lastRendered[i].transform.position + new Vector3(0, 0, -tileSize);
                if (!IsOccupied(newPosition))
                {
                    toRender.Add(Instantiate(backTilesWithProbability[Random.Range(0, backTilesWithProbability.Length)], newPosition, Quaternion.identity));
                }
            }
            if (lastRendered[i].HasTag(toLeft))
            {
                Vector3 newPosition = lastRendered[i].transform.position + new Vector3(-tileSize, 0, 0);
                if (!IsOccupied(newPosition))
                {
                    toRender.Add(Instantiate(leftTilesWithProbability[Random.Range(0, leftTilesWithProbability.Length)], newPosition, Quaternion.identity));
                }
            }
            if (lastRendered[i].HasTag(toRight))
            {
                Vector3 newPosition = lastRendered[i].transform.position + new Vector3(tileSize, 0, 0);
                if (!IsOccupied(newPosition))
                {
                    toRender.Add(Instantiate(rightTilesWithProbability[Random.Range(0, rightTilesWithProbability.Length)], newPosition, Quaternion.identity));
                }
            }
        }
        lastRendered.Clear();
        lastRendered.AddRange(toRender);
        toRender.Clear();
        generated = false;
    }

    bool IsOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, new Vector3(tileSize /3, tileSize /3, tileSize /3));
        return colliders.Length > 0;
    }
    // just tryna see how big the overlap box is.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(tileSize / 3, tileSize / 3, tileSize / 3));
    }

    void ProbabilityAdder(GameObject[] tiles, int[] probabilities, ref GameObject[] result)
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
