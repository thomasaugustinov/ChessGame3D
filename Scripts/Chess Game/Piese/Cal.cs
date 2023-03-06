using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cal : Piesa
{
    Vector2Int[] offseturi = new Vector2Int[]
    {
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(-2, 1),
        new Vector2Int(-2, -1),
        new Vector2Int(-1, 2),
        new Vector2Int(-1, -2),
    };
    public override List<Vector2Int> SelectarePatratePosibile()
    {
        mutariPosibile.Clear();
        for(int i = 0; i < offseturi.Length; i++)
        {
            Vector2Int coordUrmatoare = patratOcupat + offseturi[i];
            Piesa piesa = tabla.PiesaPatrat(coordUrmatoare);
            if (!tabla.VerificareDacaCoordonateleSuntPeTabla(coordUrmatoare))
                continue;
            if(piesa == null || !piesa.AreAceeasiCuloare(this))
                IncercareAdaugareMutare(coordUrmatoare);
        }
        return mutariPosibile;
    }
}
