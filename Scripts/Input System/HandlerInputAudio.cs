using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerInputAudio : MonoBehaviour
{
    public static HandlerInputAudio instanta;
    void Awake()
    {
        if (instanta == null)
            instanta = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(transform.gameObject);
    }
}
