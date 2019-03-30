using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTest : MonoBehaviour
{
    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    // Start is called before the first frame update
    void Start()
    {
        //ESSENTIAL SETUP
        if (LayerMask.NameToLayer("Node") == (LayerMask.NameToLayer("Node") | (1 << 11)))
        {
            Debug.LogError("No Layer Node, we need a Node layer, and every node to be tagged node");
        }
        if (LayerMask.NameToLayer("Object") == (LayerMask.NameToLayer("Object") | (1 << 12)))
        {
            Debug.LogError("No Layer Object, we need an object layer, and every obstical to be tagged Object");
        }

        //PATHFINDER TESTING
        if (!GameObject.Find("Pathfinder"))
       {
            Debug.LogError("No Pathfinder gameobject found; do you have an object called 'Pathfinder' in the scene with a valid" +
            "Pathfinder script attached to it?");
       }
       if (!GameObject.Find("Pathfinder").GetComponent<Pathfinder>())
       {
            Debug.LogError("No Pathfinder script found; You need to attach a 'Pathfinder' script to the gameobject called Pathfinder");

       }
     

       //GRID TESTING
        if (!GameObject.Find("Pathfinder").GetComponent<Grid>())
        {
            Debug.LogError("No Grid script found; You need to attach a 'Grid' script to the gameobject called Pathfinder if you " +
                "wish to generate a grid of nodes");
        }
        else
        {
            if (GameObject.Find("Pathfinder").GetComponent<Grid>().gridsizeX <= 0)
            {
                Debug.LogWarning("The gridsize x is set to 0 or less, so no nodes will be generated!");
            }
            if (GameObject.Find("Pathfinder").GetComponent<Grid>().gridsizeY <= 0)
            {
                Debug.LogWarning("The gridsize y is set to 0 or less, so no nodes will be generated!");
            }
            if (GameObject.Find("Pathfinder").GetComponent<Grid>().nodeRad <= 0)
            {
                Debug.LogWarning("The node rad is set to 0 or less, so no nodes will be generated!");
            }
            if (GameObject.Find("Pathfinder").GetComponent<Grid>().NodeRadiusToCheck <= 0)
            {
                Debug.LogWarning("The node radius to check is set to 0 or less, so nodes wont find any connections!");
            }
            if (GameObject.Find("Pathfinder").GetComponent<Grid>().nodeprefab == null)
            {
                Debug.LogError("You need to create and assign a nodeprefab to the grid-  A gameobject with the layer set to 'Node', a 'Node' " +
                    "script attached, and a CircleCollider2D as a component.");
            }
            if (!GameObject.Find("Pathfinder").GetComponent<Grid>().nodeprefab.GetComponent<Node>())
            {
                Debug.LogError("No 'Node' script attached to the prefab on the Grid.");
            }
        }

        //AGENT TESTING
        if (!GameObject.FindObjectOfType<Agent>())
        {
            Debug.LogWarning("No Agents in the scene, you should create a gameobject with an agent script if you want to have a thing" +
                " follow a path!");
        }
        else
        {
            Agent[] agents = GameObject.FindObjectsOfType<Agent>();
            foreach (Agent agent in agents)
            {
                if (!agent.gameObject.GetComponent<PathMover>())
                {
                    Debug.LogError("Agent " + agent.name + " has no PathMover!");
                }
                else
                {
                    if(agent.gameObject.GetComponent<PathMover>().movespeed <= 0)
                    {
                        Debug.LogWarning("Agent " + agent.name + " has no movespeed, and wont be able to move!");
                    }
                    if (agent.gameObject.GetComponent<PathMover>().fudgeDistance <= 0)
                    {
                        Debug.LogWarning("Agent " + agent.name + " has no fudge distance, and will attempt to hit the exact point" +
                            " they are told, which can lead to unexpected behavior!");
                    }
                }
            }


        }

        //NODE TESTING
        if (!GameObject.FindObjectOfType<Node>())
        {
            Debug.LogWarning("Weird, No nodes where generated!");
        }
        else
        {
            if (GameObject.FindObjectOfType<Node>().gameObject.layer != LayerMask.NameToLayer("Node"))
            {
                Debug.LogError("Nodes are not on the layer 'Node', so wont work properly!");
            }
            //circle collider?
            if (!GameObject.FindObjectOfType<Node>().GetComponent<CircleCollider2D>())
            {
                Debug.LogError("Nodes need a CircleCollider2d component in order to detect one another");

            }
            else
            {
                if (GameObject.FindObjectOfType<Node>().GetComponent<CircleCollider2D>().isTrigger == false)
                {
                    Debug.LogWarning("Nodes are currently not triggers! recomend setting nodes to isTrigger on the circle collider " +
                        "so the player doesnt bump into them");
                }
                if (GameObject.FindObjectOfType<Node>().GetComponent<CircleCollider2D>().radius > 2.0f)
                {
                    Debug.LogWarning("Node's CircleCollider2D radius is really big - this might cause inacuracies - recomended radius around 0.1");
                }

            }
            //connections?
            if (GameObject.FindObjectOfType<Node>().GetComponent<Node>().connections.Count <= 0)
            {
                Debug.LogError("Nodes have no connections! Somthing went wrong here!");

            }

            if (GameObject.Find("Pathfinder").transform.GetChild(GameObject.Find("Pathfinder").transform.childCount /2 ).GetComponent<Node>().connections.Count <= 4)
            {
                Debug.LogWarning("Nodes are not detecting diagonal nodes, you need to increase the Node Radius To Check in Pathfinder");
            }
            if (GameObject.Find("Pathfinder").transform.GetChild(GameObject.Find("Pathfinder").transform.childCount / 2).GetComponent<Node>().connections.Count > 8)
            {
                Debug.LogWarning("Nodes are detecting to many nodes! you probably should decrease the Node Radius To Check in pathfinder");
            }



        }


    }

    
}
