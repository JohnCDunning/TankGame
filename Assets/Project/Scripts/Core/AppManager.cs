using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager _Instance;

    public ExplosionObjects[] _ExplosionObjects;

    public AppManager Instance { get => _Instance;}

    private void Awake()
    {
        _Instance = this;
    }

    public void SpawnExplosion(ExplosionSize explosionSize, Vector3 position)
    {
        for (int i = 0; i < _ExplosionObjects.Length; i++)
        {
            if(_ExplosionObjects[i]._ExplosionSize == explosionSize)
            {
                Instantiate(_ExplosionObjects[i]._ExplosionPrefab, position, Quaternion.identity);
                return;
            }
        }
    }
}

[System.Serializable]
public class ExplosionObjects
{
    public ExplosionSize _ExplosionSize;
    public GameObject _ExplosionPrefab;
}
