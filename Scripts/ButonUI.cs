using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputUIReciever))]
public class ButonUI : Button
{
    private InputReciever reciever;

    protected override void Awake()
    {
        base.Awake();
        reciever = GetComponent<InputUIReciever>();
        onClick.AddListener(() => reciever.OnInputRecieved());
    }
}
