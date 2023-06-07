using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChecker : MonoBehaviour
{
    private MoveObject moveObject;

    void Start()
    {
        moveObject = GetComponent<MoveObject>();
    }

    public void SetInitialSpeed()
    {
        moveObject.moveSpeed = 0.001f;
    }
}
