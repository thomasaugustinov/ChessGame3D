using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Promovare : MonoBehaviour
{
    [SerializeField] private GameObject UIParent;

    internal void HideUI()
    {
        UIParent.SetActive(false);
    }

    internal void ShowUI()
    {
        UIParent.SetActive(true);
    }
}
