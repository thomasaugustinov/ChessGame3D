using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TweenerObiect))]
[RequireComponent(typeof(SetterMaterial))]
public abstract class Piesa : MonoBehaviour
{
    private SetterMaterial setterMaterial;

    public Tabla tabla { protected get; set; }
    public Vector2Int patratOcupat { get; set; }
    public CuloareEchipa echipa { get; set; }
    public bool aFostMutat { get; private set; }
    public bool aFostMutatPentruPrimaData { get; set; }
    public bool aFostMutatVerificare { get; private set; }
    public List<Vector2Int> mutariPosibile;

    private TweenerObiect tweener;

    public abstract List<Vector2Int> SelectarePatratePosibile();

    private void Awake()
    {
        mutariPosibile = new List<Vector2Int>();
        tweener = GetComponent<TweenerObiect>();
        setterMaterial = GetComponent<SetterMaterial>();
        aFostMutat = false;
        aFostMutatPentruPrimaData = false;
        aFostMutatVerificare = false;
    }

    public void SetareMaterial(Material material)
    {
        if (setterMaterial == null)
            setterMaterial = GetComponent<SetterMaterial>();
        setterMaterial.SetareMaterialUnic(material);
    }

    public bool AtacaPiesaDeTipul<T>() where T : Piesa
    {
        foreach(var patrat in mutariPosibile)
        {
            if (tabla.PiesaPatrat(patrat) is T)
                return true;
        }
        return false;
    }

    public bool AreAceeasiCuloare(Piesa piesa)
    {
        return echipa == piesa.echipa;
    }

    public bool PoateMutaLa(Vector2Int coord)
    {
        return mutariPosibile.Contains(coord);
    }

    public virtual void MutaPiesa(Vector2Int coord)
    {
        Vector3 pozitieTarget = tabla.CalcularePozitieDinCoordonate(coord);
        patratOcupat = coord;
        aFostMutatVerificare = aFostMutat;
        aFostMutat = true;
        if (aFostMutat != aFostMutatVerificare)
            aFostMutatPentruPrimaData = true;
        else
            aFostMutatPentruPrimaData = false;
        tweener.MutaLa(transform, pozitieTarget);
    }

    protected void IncercareAdaugareMutare(Vector2Int coord)
    {
        mutariPosibile.Add(coord);
    }

    public void SetData(Vector2Int coord, CuloareEchipa echipa, Tabla tabla)
    {
        this.echipa = echipa;
        patratOcupat = coord;
        this.tabla = tabla;
        transform.position = tabla.CalcularePozitieDinCoordonate(coord);
    }
}
