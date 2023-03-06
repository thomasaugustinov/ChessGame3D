using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Tabla/Layout")]
public class LayoutTabla : ScriptableObject
{
    [Serializable]
    private class SetupPatrat
    {
        public Vector2Int position;
        public TipPiesa tipPiesa;
        public CuloareEchipa culoareEchipa;
    }

    [SerializeField] private SetupPatrat[] patrateTabla;

    public int NumaratoarePiese()
    {
        return patrateTabla.Length;
    }

    public Vector2Int CoordPatratLaIndex(int index)
    {
        if (patrateTabla.Length <= index)
        {
            Debug.LogError("Indexul piesei este in afara intervalului");
            return new Vector2Int(-1, -1);
        }
        return new Vector2Int(patrateTabla[index].position.x - 1, patrateTabla[index].position.y - 1);
    }

    public string NumePiesaLaIndex(int index)
    {
        if (patrateTabla.Length <= index)
        {
            Debug.LogError("Indexul piesei este in afara intervalului");
            return "";
        }
        return patrateTabla[index].tipPiesa.ToString();
    }

    public CuloareEchipa CuloareEchipaLaIndex(int index)
    {
        if (patrateTabla.Length <= index)
        {
            Debug.LogError("Indexul piesei este in afara intervalului");
            return CuloareEchipa.Negru;
        }
        return patrateTabla[index].culoareEchipa;
    }
}
