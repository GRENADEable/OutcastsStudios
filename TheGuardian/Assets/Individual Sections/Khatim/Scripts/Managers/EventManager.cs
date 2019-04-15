﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public GameObject brokenBoardSection;
    public GameObject woodenPlank;

    public GameObject[] paintingsEyes;

    public Collider ropeBreakCol;
    public float moveForce;
    public GameObject keyReference;

    void OnEnable()
    {
        PlayerControls.onRopeBreakMessage += OnRopeBreakEventReceived;
        GameManager.onPaintingsAwakeMessage += OnPaintingsAwakeMessageEventReceived;
        GameManager.onKeyMove += OnKeyMovementEventReceived;
        PlayerControls.onKeyMove += OnKeyMovementEventReceived;

        if (keyReference == null)
            Debug.LogWarning("Add Key Reference");

        if (brokenBoardSection == null)
            Debug.LogWarning("Add Borken Board Reference");

        if (woodenPlank == null)
            Debug.LogWarning("Add Wooden Plank Reference");

        if (ropeBreakCol == null)
            Debug.LogWarning("Add Rope Break Collider Reference");

        if (paintingsEyes == null)
            Debug.LogWarning("Add Paintings Reference");
    }

    void OnDisable()
    {
        PlayerControls.onRopeBreakMessage -= OnRopeBreakEventReceived;
        GameManager.onPaintingsAwakeMessage -= OnPaintingsAwakeMessageEventReceived;
        GameManager.onKeyMove -= OnKeyMovementEventReceived;
        PlayerControls.onKeyMove -= OnKeyMovementEventReceived;
    }

    void OnDestroy()
    {
        GameManager.onKeyMove -= OnKeyMovementEventReceived;
        PlayerControls.onRopeBreakMessage -= OnRopeBreakEventReceived;
        GameManager.onPaintingsAwakeMessage -= OnPaintingsAwakeMessageEventReceived;
        PlayerControls.onKeyMove -= OnKeyMovementEventReceived;
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
        Debug.Log("Paintings Awake");
    }

    void OnKeyMovementEventReceived()
    {
        keyReference.GetComponent<Rigidbody>().AddForce(keyReference.transform.up * moveForce + keyReference.transform.forward * moveForce);
    }
}
