using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreatorSelectorPatrat))]
public class Tabla : MonoBehaviour
{
    public const int Marime_Tabla = 8;

    [SerializeField] private Transform TransformarepatratStangaJos;
    [SerializeField] private float marimePatrat;
    [SerializeField] private Promovare PromovarePion;

    private Piesa[,] grid;
    private Piesa piesaSelectata;
    private ChessGameController chessController;
    private CreatorSelectorPatrat selectorPatrat;
    private CreatorPiesa creatorPiesa;
    private ChessPlayer player;
    private Vector2Int pozitiePionPromovare;

    private void Awake()
    {
        selectorPatrat = GetComponent<CreatorSelectorPatrat>();
        CreareGrid();
    }

    public void SetareDependente(ChessGameController chessController)
    {
        this.chessController = chessController;
    }

    private void CreareGrid()
    {
        grid = new Piesa[Marime_Tabla, Marime_Tabla];
    }

    public Vector3 CalcularePozitieDinCoordonate(Vector2Int coord)
    {
        return TransformarepatratStangaJos.position + new Vector3(coord.x * marimePatrat, 0f, coord.y * marimePatrat);
    }

    private Vector2Int CalculareCooronateDinPozitie(Vector3 pozitieInput)
    {
        int x = Mathf.FloorToInt(pozitieInput.x / marimePatrat) + Marime_Tabla / 2;
        int y = Mathf.FloorToInt(pozitieInput.z / marimePatrat) + Marime_Tabla / 2;
        return new Vector2Int(x, y);
    }

    public void LaPatratSelectat(Vector3 pozitieInput)
    {
        if (!chessController.EsteJoculInDesfasurare())
            return;
        Vector2Int coord = CalculareCooronateDinPozitie(pozitieInput);
        Piesa piesa = PiesaPatrat(coord);

        if (piesaSelectata)
        {
            if (piesa != null && piesaSelectata == piesa)
            {
                DeselectarePiesa();
            }
            else if (piesa != null && piesaSelectata != piesa && chessController.EsteRandulEchipeiActive(piesa.echipa))
                SelectarePiesa(piesa);
            else if (piesaSelectata.PoateMutaLa(coord))
            {
                LaPiesaSelectataMutata(coord, piesaSelectata);
            }
        }
        else
        {
            if (piesa != null && chessController.EsteRandulEchipeiActive(piesa.echipa))
                SelectarePiesa(piesa);
        }
    }

    private void SelectarePiesa(Piesa piesa)
    {
        chessController.EliminaMutariCarePermitAtacareaPiesei<Rege>(piesa);
        piesaSelectata = piesa;
        List<Vector2Int> selectie = piesaSelectata.mutariPosibile;
        ArataPatrateSelectie(selectie);
    }

    private void ArataPatrateSelectie(List<Vector2Int> selectie)
    {
        Dictionary<Vector3, bool> dataPatrate = new Dictionary<Vector3, bool>();
        for(int i = 0; i < selectie.Count; i++)
        {
            Vector3 pozitie = TransformarepatratStangaJos.position + new Vector3(selectie[i].x * marimePatrat - 0.588f, 0f, selectie[i].y * marimePatrat - 0.59f);
            bool estePatratulNeocupat = PiesaPatrat(selectie[i]) == null;
            if (selectie[i] == piesaSelectata.patratOcupat + new Vector2Int(1, 1) && PiesaPatrat(selectie[i]) == null && piesaSelectata.echipa == CuloareEchipa.Alb && piesaSelectata.patratOcupat.y == 4 && piesaSelectata is Pion)
                estePatratulNeocupat = false;
            if (selectie[i] == piesaSelectata.patratOcupat + new Vector2Int(-1, 1) && PiesaPatrat(selectie[i]) == null && piesaSelectata.echipa == CuloareEchipa.Alb && piesaSelectata.patratOcupat.y == 4 && piesaSelectata is Pion)
                estePatratulNeocupat = false;
            if (selectie[i] == piesaSelectata.patratOcupat + new Vector2Int(-1, -1) && PiesaPatrat(selectie[i]) == null && piesaSelectata.echipa == CuloareEchipa.Negru && piesaSelectata.patratOcupat.y == 3 && piesaSelectata is Pion)
                estePatratulNeocupat = false;
            if (selectie[i] == piesaSelectata.patratOcupat + new Vector2Int(1, -1) && PiesaPatrat(selectie[i]) == null && piesaSelectata.echipa == CuloareEchipa.Negru && piesaSelectata.patratOcupat.y == 3 && piesaSelectata is Pion)
                estePatratulNeocupat = false;
            dataPatrate.Add(pozitie, estePatratulNeocupat);
        }
        selectorPatrat.ArataSelectie(dataPatrate);
    }

    public void DeselectarePiesa()
    {
        piesaSelectata = null;
        selectorPatrat.StergereSelectie();
    }

    public void LaPiesaSelectataMutata(Vector2Int coord, Piesa piesa)
    {
        IncercareCapturarePiesa(coord);
        UpdateTablaLaMutarePiesa(coord, piesa.patratOcupat, piesa, null);
        piesaSelectata.MutaPiesa(coord);
        if (piesa is Pion)
        {
            if (coord.y == 0 || coord.y == 7)
            {
                selectorPatrat.StergereSelectie();
                DeselectarePiesa();
            }
            else
            {
                DeselectarePiesa();
                EndTurn();
            }
        }
        else
        { 
            DeselectarePiesa();
            EndTurn();
        }
    }

    public void PromovareShow()
    {
        PromovarePion.ShowUI();
    }

    private void IncercareCapturarePiesa(Vector2Int coord)
    {
        Piesa piesa = PiesaPatrat(coord);
        Piesa piesaEnPassant;
        if (piesaSelectata.echipa == CuloareEchipa.Alb)
            piesaEnPassant = PiesaPatrat(coord + Vector2Int.down);
        else
            piesaEnPassant = PiesaPatrat(coord + Vector2Int.up);
        if (piesa != null && !piesaSelectata.AreAceeasiCuloare(piesa))
            CapturarePiesa(piesa);
        if (piesa is null && piesaEnPassant is Pion && !piesaSelectata.AreAceeasiCuloare(piesaEnPassant) && piesaSelectata.mutariPosibile.Contains(coord))
        { 
            CapturarePiesa(piesaEnPassant);
        }
    }

    public void CapturarePiesa(Piesa piesa)
    {
        if(piesa)
        {
            grid[piesa.patratOcupat.x, piesa.patratOcupat.y] = null;
            chessController.LaPiesaCapturata(piesa);
        }
    }

    public void EndTurn()
    {
        chessController.EndTurn();
    }

    public void UpdateTablaLaMutarePiesa(Vector2Int coordNoi, Vector2Int coordVechi, Piesa piesaNoua, Piesa piesaVeche)
    {
        grid[coordVechi.x, coordVechi.y] = piesaVeche;
        grid[coordNoi.x, coordNoi.y] = piesaNoua;
    }


    public Piesa PiesaPatrat(Vector2Int coord)
    {
        if (VerificareDacaCoordonateleSuntPeTabla(coord))
            return grid[coord.x, coord.y];
        return null;
    }

    public bool VerificareDacaCoordonateleSuntPeTabla(Vector2Int coord)
    {
        if (coord.x < 0 || coord.y < 0 || coord.x >= Marime_Tabla || coord.y >= Marime_Tabla)
            return false; 
        return true;
    }

    internal bool ArePiesa(Piesa piesa)
    {
        for (int i = 0; i < Marime_Tabla; i++)
        {
            for (int j = 0; j < Marime_Tabla; j++)
            {
                if (grid[i, j] == piesa)
                    return true;
            }
        }
        return false;
    }

    public void PunerePiesaPeTabla(Vector2Int coord, Piesa piesa)
    {
        if (VerificareDacaCoordonateleSuntPeTabla(coord))
            grid[coord.x, coord.y] = piesa;
    }

    internal void LaJocRestartat()
    {
        selectorPatrat.StergereSelectie();
        piesaSelectata = null;
        CreareGrid();
    }
}
