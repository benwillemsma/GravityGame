using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoverType
{
    Wall,
    Wall_Right,
    Wall_Left,
    Barrier,
    Barrier_Right,
    Barrier_Left,
    Spot,
    Column,
}

[RequireComponent(typeof(BoxCollider))]
public class Cover : MonoBehaviour
{
    public CoverType coverType;

    public Cover Next_Right;
    public Cover Next_Left;

    private new BoxCollider collider;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    public void MoveToNextCover(Cover next)
    {

    }
}
