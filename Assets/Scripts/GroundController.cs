using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundController : MonoBehaviour
{
    void Start()
    {
        // Tag the ground for collision detection
        gameObject.tag = "Ground";
    }
}