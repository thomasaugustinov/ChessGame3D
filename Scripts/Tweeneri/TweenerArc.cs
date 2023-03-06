using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenerArc : MonoBehaviour, TweenerObiect
{
    [SerializeField] private float viteza;
    [SerializeField] private float inaltime;

    public  void MutaLa(Transform transform, Vector3 pozitieTarget)
    {
        float distanta = Vector3.Distance(pozitieTarget, transform.position);
        transform.DOJump(pozitieTarget, inaltime, 1, distanta / viteza);
    }

}
