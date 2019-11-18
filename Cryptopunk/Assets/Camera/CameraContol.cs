using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContol : MonoBehaviour
{
    [SerializeField] float maxPan = 12f;
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] float panSpeed = 3f;
    [SerializeField] float zoomSpeed = 1f;
    [SerializeField] float zoomMax = 12f;
    [SerializeField] float zoomMin = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Pan();
        Zoom();
        Rotate();
    }

    private void Rotate()
    {
        if(Input.GetKey(KeyCode.Q))
        {
            gameObject.transform.Rotate(new Vector3(0f,rotateSpeed*Time.deltaTime,0f));
        }
        else if(Input.GetKey(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f,-1f*rotateSpeed * Time.deltaTime, 0f));
        }
    }

    private void Zoom()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            float zoom = Mathf.Clamp(gameObject.transform.localScale.x - zoomSpeed, zoomMin, zoomMax);
            gameObject.transform.localScale = new Vector3(zoom, zoom, zoom);
        }
        else if (Input.GetKey(KeyCode.X))
        {
            float zoom = Mathf.Clamp(gameObject.transform.localScale.x + zoomSpeed, zoomMin, zoomMax);
            gameObject.transform.localScale = new Vector3(zoom, zoom, zoom);
        }
    }

    private void Pan()
    {
        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.position += gameObject.transform.rotation * Vector3.forward * panSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.position += gameObject.transform.rotation * Vector3.back * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.position += gameObject.transform.rotation * Vector3.left * panSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.position += gameObject.transform.rotation * Vector3.right * panSpeed * Time.deltaTime;
        }
        if (gameObject.transform.position.magnitude > maxPan)
        {
            gameObject.transform.position = gameObject.transform.position.normalized * maxPan;
        }
    }
}
