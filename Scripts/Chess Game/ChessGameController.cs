using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CreatorPiesa))]
public class ChessGameController : MonoBehaviour
{
    private enum StatutJoc { Inceput, Jucare, Sfarsit}

    [SerializeField] private LayoutTabla layoutTablaInceput;
    [SerializeField] private Tabla tabla;
    [SerializeField] private ChessUIManager UIManager;
    [SerializeField] private Promovare PromovarePion;

    private CreatorPiesa creatorPiesa;
    private ChessPlayer playerAlb;
    private ChessPlayer playerNegru;
    private ChessPlayer playerActiv;
    private Type tipPiesaPromovare;
    private Vector2Int pozitiePromovare;
    private Piesa piesaPromovare;
    private Piesa[] pieseAtacatoare;

    private StatutJoc statut;

    private void Awake()
    {
        SetareDependente();
        CrearePlayeri();
    }

    private void SetareDependente()
    {
        creatorPiesa = GetComponent<CreatorPiesa>();
    }

    private void CrearePlayeri()
    {
        playerAlb = new ChessPlayer(CuloareEchipa.Alb, tabla);
        playerNegru = new ChessPlayer(CuloareEchipa.Negru, tabla);
    }

    private void Start()
    {
        StartJocNou();
    }

    private void StartJocNou()
    {
        Camera.main.transform.position = new Vector3(0f, 14f, -11.5f);
        Camera.main.transform.rotation = Quaternion.Euler(55, 0, 0);
        SetareStatutJoc(StatutJoc.Inceput);
        UIManager.HideUI();
        PromovarePion.HideUI();
        tabla.SetareDependente(this);
        CrearePieseDinLayout(layoutTablaInceput);
        playerActiv = playerAlb;
        GenerareMutariPosibileAleJucatorului(playerActiv);
        SetareStatutJoc(StatutJoc.Jucare);
    }

    public void RestartJoc()
    {
        if(playerActiv == playerNegru && statut == StatutJoc.Jucare)
            Camera.main.GetComponent<Animation>().Play("RotireCameraNegru");
        DistrugerePiese();
        tabla.LaJocRestartat();
        playerAlb.LaJocRestartat();
        playerNegru.LaJocRestartat();
        StartJocNou();
    }

    private void DistrugerePiese()
    {
        playerAlb.pieseActive.ForEach(p => Destroy(p.gameObject));
        playerNegru.pieseActive.ForEach(p => Destroy(p.gameObject));
    }

    private void SetareStatutJoc(StatutJoc statut)
    {
        this.statut = statut;
    }

    public bool EsteJoculInDesfasurare()
    {
        return statut == StatutJoc.Jucare;
    }

    private void CrearePieseDinLayout(LayoutTabla layout)
    {
        for (int i = 0; i < layout.NumaratoarePiese(); i++)
        {
            Vector2Int coordPatrat = layout.CoordPatratLaIndex(i);
            CuloareEchipa echipa = layout.CuloareEchipaLaIndex(i);
            string tipNume = layout.NumePiesaLaIndex(i);

            Type tip = Type.GetType(tipNume);
            CrearePiesaSiInitializare(coordPatrat, echipa, tip);
        }
    }

    public void CrearePiesaSiInitializare(Vector2Int coordPatrat, CuloareEchipa echipa, Type tip)
    {
        Piesa piesaNoua = creatorPiesa.CrearePiesa(tip).GetComponent<Piesa>();
        piesaNoua.SetData(coordPatrat, echipa, tabla);

        Material materialEchipa = creatorPiesa.MaterialEchipa(echipa);
        piesaNoua.SetareMaterial(materialEchipa);

        tabla.PunerePiesaPeTabla(coordPatrat, piesaNoua);
        ChessPlayer playerCurent = echipa == CuloareEchipa.Alb ? playerAlb : playerNegru;
        playerCurent.AdaugaPiesa(piesaNoua);

        if (piesaNoua is Cal && piesaNoua.echipa == CuloareEchipa.Negru)
            piesaNoua.gameObject.transform.Rotate(0, 180, 0);
    }

    private void GenerareMutariPosibileAleJucatorului(ChessPlayer player)
    {
        player.GenerareMutariPosibile();
    }

    public bool EsteRandulEchipeiActive(CuloareEchipa echipa)
    {
        return playerActiv.echipa == echipa;
    }

    public void PromovarePiesa(int type)
    {
        PromovarePion.HideUI();
        switch (type)
        {
            case Constante.Piesa_Regina:
                tipPiesaPromovare = typeof(Regina);
                break;
            case Constante.Piesa_Turn:
                tipPiesaPromovare = typeof(Turn);
                break;
            case Constante.Piesa_Nebun:
                tipPiesaPromovare = typeof(Nebun);
                break;
            case Constante.Piesa_Cal:
                tipPiesaPromovare = typeof(Cal);
                break;
        }
    }

    public void SchimbarePiesaPromovare()
    {
        AflarePionPromovare();
        if (piesaPromovare is Pion)
        {
            tabla.CapturarePiesa(piesaPromovare);
            CrearePiesaSiInitializare(piesaPromovare.patratOcupat, piesaPromovare.echipa, tipPiesaPromovare);
        }
    }


    public void AflarePionPromovare()
    {
        pozitiePromovare.y = 7;
        int i;
        for (i = 0; i < 8; i++)
        {
            pozitiePromovare.x = i;
            piesaPromovare = tabla.PiesaPatrat(pozitiePromovare);
            if (piesaPromovare is Pion)
                break;
        }
        pozitiePromovare.y = 0;
        for (i = 0; i < 8; i++)
        {
            pozitiePromovare.x = i;
            if (piesaPromovare is Pion)
                break;
            piesaPromovare = tabla.PiesaPatrat(pozitiePromovare);
            if (piesaPromovare is Pion)
                break;
        }
        
    }

    public void EndTurn()
    {
        SchimbarePiesaPromovare();
        GenerareMutariPosibileAleJucatorului(playerActiv);
        GenerareMutariPosibileAleJucatorului(PlayerOponent(playerActiv));
        if (VerificareSfarsitJoc())
            SfarsitJoc();
        else
        {
            SchimbaEchipaActiva();
            if (playerActiv == playerNegru)
                Camera.main.GetComponent<Animation>().Play("RotireCameraAlb");
            else
                Camera.main.GetComponent<Animation>().Play("RotireCameraNegru");
        }
    }

    private bool VerificareSfarsitJoc()
    {
        pieseAtacatoare = playerActiv.PieseCareAtacaPieseOponenteDeTipul<Rege>();
        if (pieseAtacatoare.Length > 0)
        {
            ChessPlayer playerOponent = PlayerOponent(playerActiv);
            Piesa regeAtacat = playerOponent.PrimestePieseDeTipul<Rege>().FirstOrDefault();
            playerOponent.EliminaMutariCarePermitAtacareaPiesei<Rege>(playerActiv, regeAtacat);

            int mutariPosibileAleRegelui = regeAtacat.mutariPosibile.Count;
            if (mutariPosibileAleRegelui == 0)
            {
                bool poateAparaRegele = playerOponent.PoateAparaPiesaDeAtac<Rege>(playerActiv);
                if (!poateAparaRegele)
                    return true;
            }
        }
        else
        {
            ChessPlayer playerOponent = PlayerOponent(playerActiv);
            int nr = 0;
            foreach (var piesa in playerOponent.pieseActive)
            {
                nr = nr + piesa.mutariPosibile.Count;
            }
            List<Vector2Int> coordPentruEliminare = new List<Vector2Int>();
            foreach (var piesa in playerOponent.pieseActive)
            {
                foreach (var coord in piesa.mutariPosibile)
                {
                    Piesa piesaPePatrat = tabla.PiesaPatrat(coord);
                    tabla.UpdateTablaLaMutarePiesa(coord, piesa.patratOcupat, piesa, null);
                    playerActiv.GenerareMutariPosibile();
                    if (playerActiv.VerificaDacaOponentAtacaPiesa<Rege>())
                        coordPentruEliminare.Add(coord);
                    tabla.UpdateTablaLaMutarePiesa(piesa.patratOcupat, coord, piesa, piesaPePatrat);
                }
            }
            if (nr == coordPentruEliminare.Count)
                return true;

            if (playerActiv.pieseActive.Count == 2 && playerOponent.pieseActive.Count == 1)
            {
                if (playerActiv.pieseActive[0] is Nebun || playerActiv.pieseActive[1] is Nebun)
                    return true;
                else if (playerActiv.pieseActive[0] is Cal || playerActiv.pieseActive[1] is Cal)
                    return true;
            }
            else if (playerActiv.pieseActive.Count == 1 && playerOponent.pieseActive.Count == 2)
            {
                if (playerOponent.pieseActive[0] is Nebun || playerOponent.pieseActive[1] is Nebun)
                    return true;
                else if (playerOponent.pieseActive[0] is Cal || playerOponent.pieseActive[1] is Cal)
                    return true;
            }
            else if (playerActiv.pieseActive.Count == 1 && playerOponent.pieseActive.Count == 1)
                return true;
        }
        return false;
    }

    public void LaPiesaCapturata(Piesa piesa)
    {
        ChessPlayer detinatorPiesa = (piesa.echipa == CuloareEchipa.Alb) ? playerAlb : playerNegru;
        detinatorPiesa.StergePiesa(piesa);
        Destroy(piesa.gameObject);
    }

    private void SfarsitJoc()
    {
        SetareStatutJoc(StatutJoc.Sfarsit);
        ChessPlayer playerOponent = PlayerOponent(playerActiv);
        
        int nr = 0;
        foreach (var piesa in playerOponent.pieseActive)
        {
            nr = nr + piesa.mutariPosibile.Count();
        }
        List<Vector2Int> coordPentruEliminare = new List<Vector2Int>();
        foreach (var piesa in playerOponent.pieseActive)
        {
            foreach (var coord in piesa.mutariPosibile)
            {
                Piesa piesaPePatrat = tabla.PiesaPatrat(coord);
                tabla.UpdateTablaLaMutarePiesa(coord, piesa.patratOcupat, piesa, null);
                playerActiv.GenerareMutariPosibile();
                if (playerActiv.VerificaDacaOponentAtacaPiesa<Rege>())
                    coordPentruEliminare.Add(coord);
                tabla.UpdateTablaLaMutarePiesa(piesa.patratOcupat, coord, piesa, piesaPePatrat);
            }
        }
        Piesa regeAtacat = playerOponent.PrimestePieseDeTipul<Rege>().FirstOrDefault();
        playerOponent.EliminaMutariCarePermitAtacareaPiesei<Rege>(playerActiv, regeAtacat);

        int nrMaterialInsuficient = 0;
        if (playerActiv.pieseActive.Count == 2 && playerOponent.pieseActive.Count == 1)
        {
            if (playerActiv.pieseActive[0] is Nebun || playerActiv.pieseActive[1] is Nebun)
                nrMaterialInsuficient = 1;
            else if (playerActiv.pieseActive[0] is Cal || playerActiv.pieseActive[1] is Cal)
                nrMaterialInsuficient = 1;
        }
        else if (playerActiv.pieseActive.Count == 1 && playerOponent.pieseActive.Count == 2)
        {
            if (playerOponent.pieseActive[0] is Nebun || playerOponent.pieseActive[1] is Nebun)
                nrMaterialInsuficient = 1;
            else if (playerOponent.pieseActive[0] is Cal || playerOponent.pieseActive[1] is Cal)
                nrMaterialInsuficient = 1;
        }
        else if (playerActiv.pieseActive.Count == 1 && playerOponent.pieseActive.Count == 1)
            nrMaterialInsuficient = 1;
        if (nr == coordPentruEliminare.Count && pieseAtacatoare.Length == 0)
        {
            UIManager.LaJocIncheiat("Remiză");
        }
        else if (nrMaterialInsuficient == 1)
        { 
            UIManager.LaJocIncheiat("Remiză");
        }
        else
            UIManager.LaJocIncheiat(playerActiv.echipa.ToString());
    }

    public void SchimbaEchipaActiva()
    {
        playerActiv = playerActiv == playerAlb ? playerNegru : playerAlb;
    }

    private ChessPlayer PlayerOponent(ChessPlayer player)
    {
        return player == playerAlb ? playerNegru : playerAlb;
    }

    public void EliminaMutariCarePermitAtacareaPiesei<T>(Piesa piesa) where T : Piesa
    {
        playerActiv.EliminaMutariCarePermitAtacareaPiesei<T>(PlayerOponent(playerActiv), piesa);
    }
}
