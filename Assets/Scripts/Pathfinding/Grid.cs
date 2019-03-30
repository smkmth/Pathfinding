using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//builds a grid of nodes to a choosen size and resolution.
public class Grid : MonoBehaviour {

    //node prefab object.
    public GameObject nodeprefab;

    //how big should the grid actually be
    public float gridsizeX;
    public float gridsizeY;

    //the size of the nodes used
    private float nodeDiameter;
    public float nodeRad;

    //how far each node should check
    public float NodeRadiusToCheck;



    private Pathfinder pathfinder;


	// Use this for initialization
	void Awake () {

        List<Node> nodes = new List<Node>();

        pathfinder = GetComponent<Pathfinder>();

        //get diameter from radius
        nodeDiameter = nodeRad * 2;

        //get the size of the grid accomidating for node size 
        gridsizeX = Mathf.RoundToInt(gridsizeX / nodeDiameter);
        gridsizeY= Mathf.RoundToInt(gridsizeY / nodeDiameter);
        
        //convert centerpos into a vector2
        Vector2 centerpos = new Vector2(transform.position.x, transform.position.y);

        //get the location of the bottomleft of the grid baised on this gameobject transform
        Vector2 worldBottomLeft = centerpos - Vector2.right * gridsizeX / 2 - Vector2.up * gridsizeY / 2;

        //double sized array to build the actual grid
        for(int x=0; x <= gridsizeX; x++)
        {
            for (int y = 0; y <= gridsizeY; y++)
            {

                GameObject nodeobj = Instantiate(nodeprefab, transform);
                Vector2 gridPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRad) + Vector2.up * (y *nodeDiameter + nodeRad);
                nodeobj.transform.position = gridPoint;
                nodeobj.SetActive(true);
                Node node = nodeobj.GetComponent<Node>();
                node.name = x.ToString() + " " + y.ToString();
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

    public float MaxSize
    {
        get
        {
            return gridsizeX * gridsizeY;
        }
    }
	

}
