﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // public delegate void GeneralEvent();
    // public event GeneralEvent myGeneralEvent;

    // public void CallMyGeneralEvent()
    // {
    //     if (myGeneralEvent != null)
    //     {
    //         myGeneralEvent();
    //     }
    // }

    public GameObject brokenBoardSection;
    public GameObject woodenPlank;

    public GameObject[] paintingsEyes;

    public Collider ropeBreakCol;

    void OnEnable()
    {
        PlayerControls.onRopeBreakMessage += OnRopeBreakEventReceived;
        GameManager.onPaintingsAwakeMessage += OnPaintingsAwakeMessageEventReceived;
    }

    void OnDisable()
    {
        PlayerControls.onRopeBreakMessage -= OnRopeBreakEventReceived;
        GameManager.onPaintingsAwakeMessage -= OnPaintingsAwakeMessageEventReceived;
    }
    void OnRopeBreakEventReceived()
    {
        if (ropeBreakCol != null)
        {
            Destroy(ropeBreakCol.GetComponent<HingeJoint>());
            Destroy(ropeBreakCol.GetComponent<Collider>());
            brokenBoardSection.SetActive(false);
            woodenPlank.SetActive(false);
            Debug.LogWarning("Rope Broken");
        }
    }

    void OnPaintingsAwakeMessageEventReceived()
    {
        for (int i = 0; i < paintingsEyes.Length; i++)
        {
            paintingsEyes[i].SetActive(true);
        }
        Debug.LogWarning("Paintings Awake");
    }
}
