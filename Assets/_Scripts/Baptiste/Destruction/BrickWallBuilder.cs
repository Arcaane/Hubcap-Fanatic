using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrickSettings
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
    public List<BrickSettings> bricks = new List<BrickSettings>();
    public List<GameObject> brickInstantiate = new List<GameObject>();
    public GameObject brickPrefab;

    public void AddBrick()
    {
        bricks.Add(new BrickSettings());
    }

    public void BuildWall()
    {
        ClearChildren();

        for (int x = 0; x < wallWidth; x++)
        {
            for (int y = 0; y < wallHeight; y++)
            {
                foreach (BrickSettings brick in bricks)
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


