using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class InputReciever : MonoBehaviour
{
    protected IInputHandler[] inputHandleri;

    public abstract void OnInputRecieved();

    private void Awake()
    {
        inputHandleri = GetComponents<IInputHandler>();
    }
}
