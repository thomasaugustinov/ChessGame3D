using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandler
{
    void ProcesareInput(Vector3 pozitieInput, GameObject obiectSelectat, Action callback);
}
