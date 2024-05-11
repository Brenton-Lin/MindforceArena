using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    public Radial radial;
    public RadialPiece radialPiecePrefab;
    public float GapWidthDegree = 1f;
    protected RadialPiece[] pieces;

    public bool isSelecting = false;
    public float angle = 0f;

    private void Start()
    {
        // Length of each radial as part of the whole circle
        var stepLength = 360f / radial.elements.Length;
        var iconDist = Vector3.Distance(radialPiecePrefab.icon.transform.position, radialPiecePrefab.piece.transform.position);

        pieces = new RadialPiece[radial.elements.Length];

        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i] = Instantiate(radialPiecePrefab, transform);

            pieces[i].transform.localPosition = Vector3.zero;
            pieces[i].transform.localRotation = Quaternion.identity;

            pieces[i].piece.fillAmount = 1f / radial.elements.Length - GapWidthDegree / 360f;
            pieces[i].piece.transform.localPosition = Vector3.zero;
            pieces[i].piece.transform.localRotation = Quaternion.Euler(0, 0, -stepLength * i);
            pieces[i].piece.color = new Color(1, 1, 1, 0.5f);

            pieces[i].icon.transform.localPosition = pieces[i].piece.transform.localPosition + Quaternion.AngleAxis(360 - (i * stepLength + stepLength / 2), Vector3.forward) * Vector3.up * iconDist;
            pieces[i].icon.sprite = radial.elements[i].Icon;
        }
    }

    public int OnJoystickRelease()
    {
        if (!isSelecting) return -1;
        isSelecting = false;
        var stepLength = 360f / radial.elements.Length;
        angle = (angle + 360) % 360;
        var activeElement = (int)(angle / stepLength);
        return activeElement;
    }

    private void Update()
    {
        var stepLength = 360f / radial.elements.Length;
        angle = (angle + 360) % 360;
        var activeElement = (int)(angle / stepLength);

        if (!isSelecting)
        { activeElement = -1; }

        for (int i = 0; i < radial.elements.Length; i++)
        {
            if (i == activeElement)
            {
                pieces[i].piece.color = new Color(1, 1, 1, 1);
            }
            else
            {
                pieces[i].piece.color = new Color(1, 1, 1, 0.5f);
            }
        }
    }
}