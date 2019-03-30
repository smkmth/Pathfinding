using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstruction : MonoBehaviour {

    public List<Node> ObstructedNodes; 
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

        Vector2 bounds = GetComponent<BoxCollider2D>().bounds.size * 1.1f;

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
