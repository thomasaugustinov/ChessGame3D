using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPlayer
{
    public CuloareEchipa echipa { get; set; }
    public Tabla tabla { get; set; }
    public List<Piesa> pieseActive { get; private set; }

    public ChessPlayer(CuloareEchipa echipa, Tabla tabla)
    {
        this.tabla = tabla;
        this.echipa = echipa;
        pieseActive = new List<Piesa>();
    }

    public void AdaugaPiesa(Piesa piesa)
    {
        if (!pieseActive.Contains(piesa))
            pieseActive.Add(piesa);
    }

    public void StergePiesa(Piesa piesa)
    {
        if (pieseActive.Contains(piesa))
            pieseActive.Remove(piesa);
    }

    public void GenerareMutariPosibile()
    {
        foreach(var piesa in pieseActive)
        {
            if (tabla.ArePiesa(piesa))
                piesa.SelectarePatratePosibile();
        }
    }

    public Piesa[] PieseCareAtacaPieseOponenteDeTipul<T>() where T : Piesa
    {
        return pieseActive.Where(p => p.AtacaPiesaDeTipul<T>()).ToArray();
    }

    public Piesa[] PrimestePieseDeTipul<T>() where T : Piesa
    {
        return pieseActive.Where(p => p is T).ToArray();
    }

    public void EliminaMutariCarePermitAtacareaPiesei<T>(ChessPlayer oponent, Piesa piesaSelectata) where T : Piesa
    {
        List<Vector2Int> coordPentruEliminare = new List<Vector2Int>();
        foreach(var coord in piesaSelectata.mutariPosibile)
        {
            Piesa piesaPePatrat = tabla.PiesaPatrat(coord);
            tabla.UpdateTablaLaMutarePiesa(coord, piesaSelectata.patratOcupat, piesaSelectata, null);
            oponent.GenerareMutariPosibile();
            if (oponent.VerificaDacaOponentAtacaPiesa<T>())
                coordPentruEliminare.Add(coord);
            tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat, coord, piesaSelectata, piesaPePatrat);
        }

        if (piesaSelectata is Rege)
        {
            foreach (var patrat in piesaSelectata.mutariPosibile)
                if (piesaSelectata.patratOcupat + Vector2Int.left * 2 == patrat)
                {
                    if (oponent.VerificaDacaOponentAtacaPiesa<T>())
                    { 
                        coordPentruEliminare.Add(piesaSelectata.patratOcupat + Vector2Int.left * 2);
                    }
                    Piesa piesaPePatrat = tabla.PiesaPatrat(piesaSelectata.patratOcupat + Vector2Int.left);
                    tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat + Vector2Int.left, piesaSelectata.patratOcupat, piesaSelectata, null);
                    oponent.GenerareMutariPosibile();
                    if (oponent.VerificaDacaOponentAtacaPiesa<T>())
                        coordPentruEliminare.Add(piesaSelectata.patratOcupat + Vector2Int.left * 2);
                    tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat, piesaSelectata.patratOcupat + Vector2Int.left, piesaSelectata, piesaPePatrat);
                }

            foreach (var patrat in piesaSelectata.mutariPosibile)
                if (piesaSelectata.patratOcupat + Vector2Int.right * 2 == patrat)
                {
                    if (oponent.VerificaDacaOponentAtacaPiesa<T>())
                    {
                        coordPentruEliminare.Add(piesaSelectata.patratOcupat + Vector2Int.right * 2);
                    }
                    Piesa piesaPePatrat = tabla.PiesaPatrat(piesaSelectata.patratOcupat + Vector2Int.right);
                    tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat + Vector2Int.right, piesaSelectata.patratOcupat, piesaSelectata, null);
                    oponent.GenerareMutariPosibile();
                    if (oponent.VerificaDacaOponentAtacaPiesa<T>())
                        coordPentruEliminare.Add(piesaSelectata.patratOcupat + Vector2Int.right * 2);
                    tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat, piesaSelectata.patratOcupat + Vector2Int.right, piesaSelectata, piesaPePatrat);
                }
        }

        foreach (var coord in coordPentruEliminare)
        {
            piesaSelectata.mutariPosibile.Remove(coord);
        }
    }

    internal void LaJocRestartat()
    {
        pieseActive.Clear();
    }

    public bool VerificaDacaOponentAtacaPiesa<T>() where T : Piesa
    {
        foreach(var piesa in pieseActive)
        {
            if (tabla.ArePiesa(piesa) && piesa.AtacaPiesaDeTipul<T>())
                return true;
        }
        return false;
    }

    public bool PoateAparaPiesaDeAtac<T>(ChessPlayer oponent) where T : Piesa
    {
        foreach(var piesa in pieseActive)
        {
            foreach(var coord in piesa.mutariPosibile)
            {
                Piesa piesaLaCoord = tabla.PiesaPatrat(coord);
                tabla.UpdateTablaLaMutarePiesa(coord, piesa.patratOcupat, piesa, null);
                oponent.GenerareMutariPosibile();
                if(!oponent.VerificaDacaOponentAtacaPiesa<T>())
                {
                    tabla.UpdateTablaLaMutarePiesa(piesa.patratOcupat, coord, piesa, piesaLaCoord);
                    return true;
                }
                tabla.UpdateTablaLaMutarePiesa(piesa.patratOcupat, coord, piesa, piesaLaCoord);
            }
        }
        return false;
    }
}
