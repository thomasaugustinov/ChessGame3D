using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputUIReciever : InputReciever
{
    [SerializeField] private UnityEvent onClick;

    public override void OnInputRecieved()
    {
        foreach (var handler in inputHandleri)
        { 
            handler.ProcesareInput(Input.mousePosition, gameObject, () => onClick.Invoke());
        }
    }
}
