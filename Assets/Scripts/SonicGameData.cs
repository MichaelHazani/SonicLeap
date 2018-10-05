using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicGameData : MonoBehaviour
{

    private static SonicGameData _instance;
    public static SonicGameData Instance { get { return _instance; } }
    private static int _ringsCollected = 30;

    private static List<GameObject> _gameEntities;
    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); }
        else { _instance = this; }

        _gameEntities = new List<GameObject>();
    }



    public void AddEntity(GameObject Entity)
    {
        _gameEntities.Add(Entity);
    }

    public void RemoveEntity(GameObject Entity)
    {
        _gameEntities.Remove(Entity);
    }

    public void ClearEntities()
    {
        if (_gameEntities.Count > 0)
        {
            foreach (GameObject go in _gameEntities)
            {
                Destroy(go);
            }
            _gameEntities.RemoveAll(delegate (GameObject go) { return go == null; });
        }
    }

    public void IncreaseRingCount()
    {
        ++_ringsCollected;
    }

    public void ResetRingCount()
    {
        _ringsCollected = 0;
    }

    public int GetRingCount()
    {
        return _ringsCollected;
    }

}
