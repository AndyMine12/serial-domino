using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableInput : MonoBehaviour
{
    public float tableInputSpeed = 0.5f;
    private Vector3 _startPos;
    private Transform _transform;

    // Start is called before the first frame update
    void Start()
    {
        this._transform = GetComponent<Transform>();
        this._startPos = this._transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("down"))
        {
            this._transform.position = this._startPos;
        }
        else
        {
            if (Input.GetKey("right"))
            {
                this._transform.position =  new Vector2( _transform.position.x + tableInputSpeed*Time.deltaTime, _transform.position.y);
            } 
            else 
            {
                if (Input.GetKey("left"))
                {
                    this._transform.position =  new Vector2( _transform.position.x - tableInputSpeed*Time.deltaTime, _transform.position.y);
                }
            }
        }

    }
}
