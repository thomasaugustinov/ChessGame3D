using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public class CreatorPiesa : MonoBehaviour
{
    [SerializeField] public GameObject[] prefabPiese;
    [SerializeField] private Material materialAlb;
    [SerializeField] private Material materialNegru;

    public Dictionary<string, GameObject> numeLaPiesaDict = new Dictionary<string, GameObject>();


    private void Awake()
    {
        foreach (var piesa in prefabPiese)
        {
            numeLaPiesaDict.Add(piesa.GetComponent<Piesa>().GetType().ToString(), piesa);
        }
    }

    public GameObject CrearePiesa(Type tip)
    {
        GameObject prefab = numeLaPiesaDict[tip.ToString()];
        if (prefab)
        {
            GameObject piesaNoua = Instantiate(prefab);
            return piesaNoua;
        }
        return null;
    }

    public Material MaterialEchipa(CuloareEchipa echipa)
    {
        return echipa == CuloareEchipa.Alb ? materialAlb : materialNegru;
    }
}
