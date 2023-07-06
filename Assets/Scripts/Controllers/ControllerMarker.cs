using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hides a temporary sprite used to identify where is a controller in the scene
public class ControllerMarker : MonoBehaviour
{
    // Upon creation, set sprite invisible
    void Start()
    {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        renderer.color = new Color(0f, 0f, 0f, 0f);
    }

}
