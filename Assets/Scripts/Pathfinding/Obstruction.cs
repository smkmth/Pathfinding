using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class Obstruction : MonoBehaviour {

    public List<Node> ObstructedNodes;
    // Declare and initialize a new List of GameObjects called currentCollisions.
    public List<GameObject> currentCollisions = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        CheckNodes();
    }

    void CheckNodes()
    {

        if (ObstructedNodes.Count != 0)
        {
            foreach (Node node in ObstructedNodes)
            {
                node.walkable = true;
            }
            ObstructedNodes.Clear();

        }

        Vector2 bounds = GetComponent<Collider2D>().bounds.size * 1.01f;

        //Vector2 bounds = new Vector2(30, 30);

        Collider2D[] results = Physics2D.OverlapBoxAll(transform.position, bounds, 0.0f);
        foreach (Collider2D result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("Node"))
            {
                ObstructedNodes.Add(result.GetComponent<Node>());

            }
        }

        foreach (Node node in ObstructedNodes)
        {
            node.walkable = false;
        }
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            CheckNodes();
        }
    }

  


}
