using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public GameObject nodeprefab;
    public List<Node> nodes;
    public float gridsizeX;
    public float gridsizeY;
    private float nodeSize;
    public float nodeRad;
    private Pathfinder pathfinder;
    public float NodeDistanceToCheck;
    public float NodeRadiusToCheck;



	// Use this for initialization
	void Start () {
        pathfinder = GetComponent<Pathfinder>();
        nodeSize = nodeRad * 2;
        gridsizeX = Mathf.RoundToInt(gridsizeX / nodeSize);
        gridsizeY= Mathf.RoundToInt(gridsizeY / nodeSize);
        Vector2 centerpos = new Vector2(transform.position.x, transform.position.y);
        Vector2 worldBottomLeft = centerpos - Vector2.right * gridsizeX / 2 - Vector2.up * gridsizeY / 2;
        for(int x=0; x <= gridsizeX; x++)
        {
            for (int y = 0; y <= gridsizeY; y++)
            {

                GameObject nodeobj = Instantiate(nodeprefab, transform);
                Vector2 gridPoint = worldBottomLeft + Vector2.right * (x * nodeSize + nodeRad) + Vector2.up * (y *nodeSize + nodeRad);
                nodeobj.transform.position = gridPoint;
                nodeobj.SetActive(true);
                Node node = nodeobj.GetComponent<Node>();
                node.name = x.ToString() + " " + y.ToString();
                node.DistanceToCheck = NodeDistanceToCheck;
                node.RadiusToCheck = NodeRadiusToCheck;
                nodes.Add(node);
            }
            
        }

        foreach(Node node in nodes)
        {
            node.CalculateNeighbors();
        }

        pathfinder.Nodes = nodes;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
