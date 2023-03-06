using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessUIManager : MonoBehaviour
{
    [SerializeField] private GameObject UIParent;
    [SerializeField] private Text rezultat;

    internal void HideUI()
    {
        UIParent.SetActive(false);
    }

    internal void LaJocIncheiat(string castigator)
    {
        UIParent.SetActive(true);
        if(castigator == "Remiză")
            rezultat.text = string.Format("Draw");
        else
        {
            if(castigator == "Alb")
                rezultat.text = string.Format("White won");
            else
                rezultat.text = string.Format("Black won");
        }
    }

}
