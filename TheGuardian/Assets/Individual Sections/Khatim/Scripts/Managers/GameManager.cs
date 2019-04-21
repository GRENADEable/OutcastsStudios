﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPlayerDead;
    public int keyCounter;
    public GameObject endHallwayDoor;

    public delegate void SendMessageToManagers();
    public static event SendMessageToManagers onPaintingsAwakeMessage;
    public static event SendMessageToManagers onKeyMove;
    public static event SendMessageToManagers onIncreaseEyeSpeed;

    void OnEnable()
    {
        KeyCollector.onKeyCounterUpdate += OnKeyDropEventReceived;
    }

    void OnDisable()
    {
        KeyCollector.onKeyCounterUpdate -= OnKeyDropEventReceived;
    }

    void OnDestroy()
    {
        KeyCollector.onKeyCounterUpdate -= OnKeyDropEventReceived;
    }

    void Update()
    {
        if (endHallwayDoor != null)
        {
            if (keyCounter >= 3)
            {
                endHallwayDoor.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                endHallwayDoor.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        if (keyCounter >= 1 && onPaintingsAwakeMessage != null)

            onPaintingsAwakeMessage();


        if (keyCounter >= 2 && onIncreaseEyeSpeed != null)
            onIncreaseEyeSpeed();
    }

    void OnKeyDropEventReceived()
    {
        keyCounter++;
        Debug.Log("Key Received");

        if (onKeyMove != null)
            onKeyMove();
    }
}
