using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tabla))]

public class HandlerInputTabla : MonoBehaviour, IInputHandler
{

    private Tabla tabla;

    private void Awake()
    {
        tabla = GetComponent<Tabla>();
    }

    public void ProcesareInput(Vector3 pozitieInput, GameObject obiectSelectat, Action callback)
    {
        tabla.LaPatratSelectat(pozitieInput);
    }
}
