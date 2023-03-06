using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TweenerLinie : MonoBehaviour, TweenerObiect
{
    [SerializeField] private float viteza;

    public void MutaLa(Transform transform, Vector3 pozitieTarget)
    {
        float distanta = Vector3.Distance(pozitieTarget, transform.position);
        transform.DOMove(pozitieTarget, distanta / viteza);
    }
}
