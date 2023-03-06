using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputColliderReciever : InputReciever
{

    private Vector3 pozitieClick;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                pozitieClick = hit.point;
                OnInputRecieved();
            }
        }
    }
    public override void OnInputRecieved()
    {
        foreach(var handler in inputHandleri)
        {
            handler.ProcesareInput(pozitieClick, null, null);
        }
    }
}
