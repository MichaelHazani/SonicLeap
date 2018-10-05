using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
public class SonicGestures : MonoBehaviour
{

    [SerializeField]
    GameObject MLSpatialMapper;
    [SerializeField]
    GameObject MeshingNodes;

    [SerializeField]
    Material InvisibleOcclusionMaterial;

    [SerializeField]
    Material WireframeMaterial;

    [SerializeField]
    LevelCreator levelCreator;

    private bool _readingHandGesture = false;
    private MLHandKeyPose[] _gestures;

    void OnEnable()
    {
        MLResult _result = MLHands.Start();
        if (!_result.IsOk)
        {
            Debug.LogError("Not running on device or ZI = no gestures");
            enabled = false;
            return;
        }
        _gestures = new MLHandKeyPose[4];
        _gestures[0] = MLHandKeyPose.OpenHandBack;
        _gestures[1] = MLHandKeyPose.L;
        _gestures[2] = MLHandKeyPose.Ok;
        _gestures[3] = MLHandKeyPose.Fist;
        MLHands.KeyPoseManager.EnableKeyPoses(_gestures, true, true);

    }
    void OnDisable()
    {
        MLHands.Stop();
    }

    void OnDestroy()
    {
        DisableMeshing();
    }

    void Update()
    {
        if (!_readingHandGesture && (MLHands.Left.HandConfidence > 0.88f && MLHands.Left.KeyPose == MLHandKeyPose.L || MLHands.Right.HandConfidence > 0.88f && MLHands.Right.KeyPose == MLHandKeyPose.L))
        {
            _readingHandGesture = true;
            EnableMeshing();
            levelCreator.ClearLevel();
        }
        else if (!_readingHandGesture && MLHands.Left.HandConfidence > 0.88f && MLHands.Left.KeyPose == MLHandKeyPose.Ok || MLHands.Right.HandConfidence > 0.88f && MLHands.Right.KeyPose == MLHandKeyPose.Ok)
        {
            _readingHandGesture = true;
            DisableMeshing();
            levelCreator.CreateLevel();
        }
        else
        {
            _readingHandGesture = false;
        }
    }

    void EnableMeshing()
    {
        foreach (Transform child in MeshingNodes.transform)
        {
            child.GetComponent<Renderer>().material = WireframeMaterial;
        }
        MLSpatialMapper.SetActive(true);
    }
    void DisableMeshing()
    {
        MLSpatialMapper.SetActive(false);
        if (MeshingNodes != null)
        {
            if (MeshingNodes.transform.childCount > 0)
            {
                foreach (Transform child in MeshingNodes.transform)
                {
                    child.GetComponent<Renderer>().material = InvisibleOcclusionMaterial;
                }
            }
        }
    }
}
