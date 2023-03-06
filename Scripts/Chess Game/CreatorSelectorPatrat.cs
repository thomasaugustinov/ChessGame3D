using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatorSelectorPatrat : MonoBehaviour
{
    [SerializeField] private Material materialPatratNeocupat;
    [SerializeField] private Material materialPatratAdversar;
    [SerializeField] private GameObject selectorPrefab;
    private List<GameObject> selectoriIstantiati = new List<GameObject>();

    public void ArataSelectie(Dictionary<Vector3, bool> datePatrat)
    {
        StergereSelectie();
        foreach(var data in datePatrat)
        {
            GameObject selector = Instantiate(selectorPrefab, data.Key, Quaternion.identity);
            selectoriIstantiati.Add(selector);
            foreach(var setter in selector.GetComponentsInChildren<SetterMaterial>())
            {
                setter.SetareMaterialUnic(data.Value ? materialPatratNeocupat : materialPatratAdversar);
            }
        }

    }

    public void StergereSelectie()
    {
        foreach (var selector in selectoriIstantiati)
            Destroy(selector.gameObject);
        selectoriIstantiati.Clear();
    }
}
