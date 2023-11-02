using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Brick
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Mesh mesh;
    public Material material;
    public GameObject prefab;
}

public class BrickWallBuilder : MonoBehaviour
{
    public int wallWidth = 10;
    public int wallHeight = 5;
    public List<Brick> bricks = new List<Brick>();
    public List<GameObject> brickInstantiate = new List<GameObject>();
    public GameObject brickPrefab;

    public void AddBrick()
    {
        bricks.Add(new Brick());
    }

    public void BuildWall()
    {
        ClearChildren();

        for (int x = 0; x < wallWidth; x++)
        {
            for (int y = 0; y < wallHeight; y++)
            {
                foreach (Brick brick in bricks)
                {
                    Vector3 position = brick.position + new Vector3(x * brick.scale.x, y * brick.scale.y, 0);
                    GameObject newBrick = Instantiate(brickPrefab, position, Quaternion.identity, transform);
                    newBrick.transform.localScale = brick.scale;
                    newBrick.transform.rotation = brick.rotation; 
                    newBrick.GetComponent<MeshFilter>().mesh = brick.mesh;
                    newBrick.GetComponent<Renderer>().material = brick.material;
                    brickInstantiate.Add(newBrick);
                }
            }
        }
    }

    public void ClearChildren()
    {
        brickInstantiate.Clear();
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}


