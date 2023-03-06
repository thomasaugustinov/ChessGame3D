using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regina : Piesa
{
    private Vector2Int[] directii = new Vector2Int[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
    };

    public override List<Vector2Int> SelectarePatratePosibile()
    {
        mutariPosibile.Clear();
        float range = Tabla.Marime_Tabla;
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
        return mutariPosibile;
    }
}
