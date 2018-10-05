using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Debugger : MonoBehaviour
{

    private static Debugger _instance;
    public static Debugger Instance { get { return _instance; } }

    public bool DebugEnemies;
    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); }
        else { _instance = this; }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame(
    void Update()
    {
#if (UNITY_EDITOR)
        if (Input.GetKeyDown(KeyCode.M))
        {
            print(Selection.activeTransform.gameObject.GetComponent<MeshFilter>().mesh.bounds.center);
            // print(Selection.activeTransform.gameObject.GetComponent<MeshFilter>().mesh.normals);
        }
#endif
    }
}
