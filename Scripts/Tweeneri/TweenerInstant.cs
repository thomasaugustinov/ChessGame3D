using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenerInstant : MonoBehaviour, TweenerObiect
{
    public void MutaLa(Transform transform, Vector3 pozitieTarget)
    {
        transform.position = pozitieTarget;
    }
}
