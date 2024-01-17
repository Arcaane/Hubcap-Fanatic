using UnityEngine;
using UnityEditor;

public class CombineMeshFilters : MonoBehaviour
{
    public MeshFilter[] meshFilters;
    public string saveFolderPath = "Assets/CombinedMeshes";

    [ContextMenu("Combine Mesh Filters")]
    void CombineMeshes()
    {
        if (!System.IO.Directory.Exists(saveFolderPath))
        {
            System.IO.Directory.CreateDirectory(saveFolderPath);
        }

        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        GameObject combinedObject = new GameObject("CombinedMesh");
        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        combinedMeshFilter.sharedMesh = new Mesh();
        combinedMeshFilter.mesh.CombineMeshes(combineInstances, true);

        string savePath = saveFolderPath + "/CombinedMesh.asset";
        AssetDatabase.CreateAsset(combinedMeshFilter.mesh, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Meshes combined and saved at: " + savePath);
    }
}