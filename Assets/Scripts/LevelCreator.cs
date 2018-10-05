using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{

    [SerializeField]
    GameObject MeshingNodes;

    [SerializeField]
    GameObject Enemy1;

    [SerializeField]
    GameObject Enemy2;

    [SerializeField]
    GameObject Enemy3;

    [SerializeField]
    GameObject Ring;
    List<GameObject> Entities;


    void Start()
    {
        Entities = new List<GameObject>();
        // GameData = GameObject.Find("GameData").GetComponent<SonicGameData>();
    }
    public void CreateLevel()
    {
        SonicGameData.Instance.ClearEntities();
        SonicGameData.Instance.ResetRingCount();
        CreateRings();
        CreateEnemies();
    }

    public void ClearLevel()
    {

        foreach (GameObject go in Entities)
        {
            Destroy(go);
        }
        Entities.RemoveAll(delegate (GameObject o) { return o == null; });
    }

    void CreateRings()
    {
        GameObject Rings = new GameObject();

        foreach (Transform meshTransform in MeshingNodes.transform)
        {
            if (meshTransform.GetComponent<MeshFilter>().mesh.bounds.center.y < 1.0f)
            {

                Vector3 overMesh = meshTransform.GetComponent<MeshFilter>().mesh.bounds.center + new Vector3(0, 0.1f, 0);
                GameObject RingInstance = Instantiate(Ring, overMesh, Quaternion.identity);
                Entities.Add(RingInstance);
                RingInstance.transform.SetParent(Rings.transform);
            }

        }

    }

    void CreateEnemies()
    {

    }
}
