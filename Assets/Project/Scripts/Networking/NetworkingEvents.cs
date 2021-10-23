using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingEvents : MonoBehaviour
{
    public static readonly byte _CreateProjectileEvent = 0;
    public static readonly byte _CreateMineEvent = 1;
    public static readonly byte _OnDestroyEvent = 2;
}
