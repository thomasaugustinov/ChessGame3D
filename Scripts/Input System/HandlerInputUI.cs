using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerInputUI : MonoBehaviour, IInputHandler
{
    public void ProcesareInput(Vector3 pozitieInput, GameObject obiectSelectat, Action callback)
    {
        callback?.Invoke();
    }
}
