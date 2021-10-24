using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimer : MonoBehaviour
{
    [SerializeField] bool AutoKill = false;
    [SerializeField] float TimeUntilDeath = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (AutoKill)
        {
            StartCoroutine(KillSelfRoutine());
        }
    }
    public void StartDeathTimer()
    {
        StartCoroutine(KillSelfRoutine());
    }
    IEnumerator KillSelfRoutine()
    {
        yield return new WaitForSeconds(TimeUntilDeath);
        Destroy(gameObject);
    }
}
