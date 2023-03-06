using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pion : Piesa
{

    public override List<Vector2Int> SelectarePatratePosibile()
    {
        mutariPosibile.Clear();
        Vector2Int directie = echipa == CuloareEchipa.Alb ? Vector2Int.up : Vector2Int.down;
        float range = aFostMutat ? 1 : 2;
        for (int i = 1; i <= range; i++)
        {
            Vector2Int coordUrmatoare = patratOcupat + directie * i;
            Piesa piesa = tabla.PiesaPatrat(coordUrmatoare);
            if (!tabla.VerificareDacaCoordonateleSuntPeTabla(coordUrmatoare))
                break;
            if (piesa == null)
                IncercareAdaugareMutare(coordUrmatoare);
            else
            {
                if (piesa.AreAceeasiCuloare(this))
                    break;
                else
                    break;
            }
        }

        int nr = 0;
        int nrEnPassant = 0;

        Vector2Int[] directiiCapturare = new Vector2Int[] { new Vector2Int(1, directie.y), new Vector2Int(-1, directie.y) };
        for (int i = 0; i < directiiCapturare.Length; i ++ )
        {
            Vector2Int coordUrmatoare = patratOcupat + directiiCapturare[i];
            Piesa piesa = tabla.PiesaPatrat(coordUrmatoare);
            if (!tabla.VerificareDacaCoordonateleSuntPeTabla(coordUrmatoare))
            {
                nr = 1;
                break;
            }
            if (piesa != null && !piesa.AreAceeasiCuloare(this))
                IncercareAdaugareMutare(coordUrmatoare);
        }

        for (int i = 0; i < directiiCapturare.Length; i++)
        {
            Vector2Int coordUrmatoare = patratOcupat + directiiCapturare[i];
            Vector2Int coordUrmatoareEnPassant = coordUrmatoare - directie;
            Piesa piesa = tabla.PiesaPatrat(coordUrmatoareEnPassant);
            if (!tabla.VerificareDacaCoordonateleSuntPeTabla(coordUrmatoare))
            {
                nrEnPassant = 1;
                break;
            }
            if (piesa != null && !piesa.AreAceeasiCuloare(this) && coordUrmatoare.y == 5 && piesa.echipa == CuloareEchipa.Negru && piesa.aFostMutatPentruPrimaData && piesa is Pion)
            { 
                IncercareAdaugareMutare(coordUrmatoare);
                piesa.aFostMutatPentruPrimaData = false;
            }
            if (piesa != null && !piesa.AreAceeasiCuloare(this) && coordUrmatoare.y == 2 && piesa.echipa == CuloareEchipa.Alb && piesa.aFostMutatPentruPrimaData && piesa is Pion)
            {
                IncercareAdaugareMutare(coordUrmatoare);
                piesa.aFostMutatPentruPrimaData = false;
            }
        }

        if (nr == 1)
        {
            Vector2Int coordUrmatoare = patratOcupat + directiiCapturare[1];
            Piesa piesa = tabla.PiesaPatrat(coordUrmatoare);
            if (piesa != null && !piesa.AreAceeasiCuloare(this))
                IncercareAdaugareMutare(coordUrmatoare);
        }

        if (nrEnPassant == 1)
        {
            Vector2Int coordUrmatoare = patratOcupat + directiiCapturare[1];
            Piesa piesa = tabla.PiesaPatrat(coordUrmatoare - directie);
            if (piesa != null && !piesa.AreAceeasiCuloare(this) && coordUrmatoare.y == 5 && piesa.echipa == CuloareEchipa.Negru && piesa.aFostMutatPentruPrimaData && piesa is Pion)
            { 
                IncercareAdaugareMutare(coordUrmatoare);
                piesa.aFostMutatPentruPrimaData = false;
            }
            if (piesa != null && !piesa.AreAceeasiCuloare(this) && coordUrmatoare.y == 2 && piesa.echipa == CuloareEchipa.Alb && piesa.aFostMutatPentruPrimaData && piesa is Pion)
            { 
                IncercareAdaugareMutare(coordUrmatoare);
                piesa.aFostMutatPentruPrimaData = false;
            }
        }

        return mutariPosibile;
    }

    public override void MutaPiesa(Vector2Int coord)
    {
        base.MutaPiesa(coord);
        VerificarePromovare();
    }

    private void VerificarePromovare()
    {
        int coordYPromovare = echipa == CuloareEchipa.Alb ? Tabla.Marime_Tabla - 1 : 0;
        if (patratOcupat.y == coordYPromovare)
        {
            tabla.PromovareShow();
        }
    }
}