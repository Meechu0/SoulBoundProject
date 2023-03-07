using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float areaSpawnRange = 5f;
    [SerializeField] private GameObject startAreaPrefab;
    [SerializeField] private List<GameObject> areaPrefabs;
    
    private List<GameObject> areaGo = new List<GameObject>();
    private List<Connector> areaConnectors = new List<Connector>();

    private void Awake()
    {
                                                               // Find the player by tag and assign.
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;     
        AddArea(startAreaPrefab);                                                                                       // Instantiate the first area.
    }

    private void Update()
    {
        foreach (var connector in areaConnectors.ToList())
        {
            if (Vector2.Distance(connector.transform.position, player.position) < areaSpawnRange       // Check how far the edge(connector position)
                && !connector.hasConnection)                                                                            // is to the player and ignore connectors that
            {                                                                                                           // already have an area next to them.
                AddArea(areaPrefabs[Random.Range(0, areaPrefabs.Count)], connector);                                // Instantiate a new random area if the edge
            }                                                                                                           // is too close.
        }
    }

    private void AddArea(GameObject area)                                                                               // Method for spawning the first area that doesn't
    {                                                                                                                   // need to mark any connectors.
        areaGo.Add(Instantiate(area, transform.position, quaternion.identity, transform));
        areaConnectors.AddRange(areaGo[areaGo.Count - 1].GetComponentsInChildren<Connector>());
    }
    
    private void AddArea(GameObject area, Connector connector)
    {
        areaGo.Add(Instantiate(area, connector.transform.position, quaternion.identity, transform));// Instantiate areas and connects and add them to 
        var connectors = areaGo[areaGo.Count - 1].GetComponentsInChildren<Connector>();                        // and add them to their respective lists.
        areaConnectors.AddRange(connectors);
        
        connector.hasConnection = true;                                                                                 // Mark the connector that an area has been instantiated next to.
        connectors[0].hasConnection = connectors[0].connectorDir != connector.connectorDir;                             // Also mark the connector of the new area
        connectors[1].hasConnection = connectors[1].connectorDir != connector.connectorDir;                             // to prevent areas spawning on top of each other.
        
        areaGo[areaGo.Count - 1].transform.position += new Vector3(AreaPosOffset(connector, connectors), 0, 0);   // Offset the position of the new area so its not overlapping.
    }

    private float AreaPosOffset(Connector connector1, Connector[] connectors)
    {                                                                                                                   // Connector2 is equal to which ever connector
        var connector2 = connectors[0].hasConnection ? connectors[0] : connectors[1];                                   // of the new area has a connection.
        return connector1.transform.position.x - connector2.transform.position.x;                                       // Return difference between the position we are spawning 
    }                                                                                                                   // the area at and the new connectors position.

    private void OnDrawGizmos()
    {
        if (!player) return;
        Gizmos.DrawWireSphere(player.position, areaSpawnRange);
    }
}
