using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HUDController : Photon.MonoBehaviour
{
    private int _EnemyTanksAlive = 0;
    [SerializeField] private TMP_Text _TanksAliveText;


    private bool _HasInit = false;

    private IEnumerator DelayedInit()
    {
        bool _Connected = false;
        
        while (_Connected == false)
        {
            if (photonView != null)
            {
                TankController[] otherTanks = GameObject.FindObjectsOfType<TankController>()
                    .Where(i => i.photonView.isSceneView).ToArray();
                
                
                _EnemyTanksAlive = otherTanks.Length;

                foreach (var tank in otherTanks)
                {
                    tank._OnDestroyed += OnDestroyed;
                }
                
                _Connected = true;
            }
            else yield return new WaitForFixedUpdate();
        }
    }

    private void Start()
    {
        StartCoroutine(DelayedInit());
    }
    

    private void OnDestroyed()
    {
        _EnemyTanksAlive--;
        _TanksAliveText.text = _EnemyTanksAlive.ToString() + " x";
    }
}
