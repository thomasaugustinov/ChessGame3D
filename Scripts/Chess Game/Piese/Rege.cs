using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rege : Piesa
{
    Vector2Int[] directii = new Vector2Int[]
    {
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1),
    };

    private Vector2Int rocadaLaStanga;
    private Vector2Int rocadaLaDreapta;

    private Piesa turnStanga;
    private Piesa turnDreapta;

    public override List<Vector2Int> SelectarePatratePosibile()
    {
        mutariPosibile.Clear();
        AplicaMutariStandard();
        AplicaMutariRocada();
        return mutariPosibile;
    }

    private void AplicaMutariRocada()
    {
        rocadaLaStanga = new Vector2Int(-1, -1);
        rocadaLaDreapta = new Vector2Int(-1, -1);
        if (!aFostMutat)
        {
            turnStanga = PrimestePiesaDinDirectia<Turn>(echipa, Vector2Int.left);
            if (turnStanga && !turnStanga.aFostMutat)
            {
                rocadaLaStanga = patratOcupat + Vector2Int.left * 2;
                mutariPosibile.Add(rocadaLaStanga);
            }

            turnDreapta = PrimestePiesaDinDirectia<Turn>(echipa, Vector2Int.right);
            if (turnDreapta && !turnDreapta.aFostMutat)
            {
                rocadaLaDreapta = patratOcupat + Vector2Int.right * 2;
                mutariPosibile.Add(rocadaLaDreapta);
            }
        }
    }

    private Piesa PrimestePiesaDinDirectia<T>(CuloareEchipa echipa, Vector2Int directie)
    {
        for (int i = 1; i <= Tabla.Marime_Tabla; i++)
        {
            Vector2Int coordUrmatoare = patratOcupat + directie * i;
            Piesa piesa = tabla.PiesaPatrat(coordUrmatoare);
            if (!tabla.VerificareDacaCoordonateleSuntPeTabla(coordUrmatoare))
                return null;
            if(piesa != null)
            {
                if (piesa.echipa != echipa || !(piesa is T))
                    return null;
                else if (piesa.echipa == echipa && piesa is T)
                    return piesa;
            }
        }
        return null;
    }

    public void AplicaMutariStandard()
    {
        float range = 1;
        foreach (var directie in directii)
        {
            for (int i = 1; i <= range; i++)
            {
                Vector2Int coordUrmatoare = patratOcupat + directie * i;
                Piesa piesa = tabla.PiesaPatrat(coordUrmatoare);
                if (!tabla.VerificareDacaCoordonateleSuntPeTabla(coordUrmatoare))
                    break;
                if (piesa == null)
                    IncercareAdaugareMutare(coordUrmatoare);
                else if (!piesa.AreAceeasiCuloare(this))
                {
                    IncercareAdaugareMutare(coordUrmatoare);
                    break;
                }
                else if (piesa.AreAceeasiCuloare(this))
                    break;
            }
        }
    }

    public override void MutaPiesa(Vector2Int coord)
    {
        base.MutaPiesa(coord);
        if (coord == rocadaLaStanga)
        {
            tabla.UpdateTablaLaMutarePiesa(coord + Vector2Int.right, turnStanga.patratOcupat, turnStanga, null);
            turnStanga.MutaPiesa(coord + Vector2Int.right);
        }
        else if (coord == rocadaLaDreapta)
        {
            tabla.UpdateTablaLaMutarePiesa(coord + Vector2Int.left, turnDreapta.patratOcupat, turnDreapta, null);
            turnDreapta.MutaPiesa(coord + Vector2Int.left);
        }
    }
}
