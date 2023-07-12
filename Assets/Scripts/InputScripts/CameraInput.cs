using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    public float cameraInputSpeed = 0.5f;

    private Transform _transform;
    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("right"))
        {
            _transform.position =  new Vector2( _transform.position.x*cameraInputSpeed*Time.deltaTime, _transform.position.y);
        } else if (Input.GetKey("left"))
        {
            _transform.position =  new Vector2( -_transform.position.x*cameraInputSpeed*Time.deltaTime, _transform.position.y);
        }


    }
}
