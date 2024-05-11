using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCylinderTrees : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] GameObject treeInQuotationMarks;

    private TreeInstance[] trees;

    // Start is called before the first frame update
    void Start()
    {
        trees = terrain.terrainData.treeInstances;

        GameObject fakeTreeForest = new GameObject();
        fakeTreeForest.name = "fakeTreeForest";

        for (int i = 0; i < terrain.terrainData.treeInstanceCount; i++)
        {

            TerrainData terrainData = terrain.terrainData;
            Vector3 treeInstancePos = trees[i].position;
            var localPos = new Vector3(treeInstancePos.x * terrainData.size.x, treeInstancePos.y * terrainData.size.y, treeInstancePos.z * terrainData.size.z);
            var worldPos = terrain.transform.TransformPoint(localPos);

            GameObject newTree = Instantiate(treeInQuotationMarks, worldPos, Quaternion.identity);
            newTree.transform.parent = fakeTreeForest.transform;

        }

    }
}
