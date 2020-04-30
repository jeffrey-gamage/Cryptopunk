using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContol : MonoBehaviour
{
    [SerializeField] internal float maxPan = 12f;
    internal Vector3 center = Vector3.zero;
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] float panSpeed = 3f;
    [SerializeField] float zoomSpeed = 1f;
    [SerializeField] float zoomMax = 12f;
    [SerializeField] float zoomMin = 3f;

    private InfiniteScrollBackground backgroundController;
    
    // Start is called before the first frame update
    void Start()
    {
        backgroundController = GetComponent<InfiniteScrollBackground>();
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
        if (Mathf.Abs(Input.GetAxis("CameraRotate")) > 0f)
        {
            gameObject.transform.Rotate(new Vector3(0f, Input.GetAxis("CameraRotate") * rotateSpeed * Time.deltaTime, 0f));
            backgroundController.Scroll(Input.GetAxis("CameraRotate") * rotateSpeed * Time.deltaTime);
        }
    }

    private void Zoom()
    {
        if(Mathf.Abs(Input.GetAxis("Zoom"))>0f)
        {
            float zoom = Mathf.Clamp(gameObject.transform.localScale.x +Input.GetAxis("Zoom")*zoomSpeed * Time.deltaTime, zoomMin, zoomMax);
            gameObject.transform.localScale = new Vector3(zoom, zoom, zoom);
        }
    }

    private void Pan()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal"))>0f)
        {
            gameObject.transform.position += gameObject.transform.rotation * Vector3.right*Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime;
        }
        if(Mathf.Abs(Input.GetAxis("Vertical"))>0f)
        {
            gameObject.transform.position += gameObject.transform.rotation * Vector3.forward *Input.GetAxis("Vertical")* panSpeed * Time.deltaTime;
        }
        if ((gameObject.transform.position-center).magnitude > maxPan)
        {
            gameObject.transform.position = center+(gameObject.transform.position-center).normalized * maxPan;
        }
    }

    internal void Configure()
    {
        gameObject.transform.position = FindObjectOfType<DeploymentZone>().transform.position;
        center = DungeonManager.instance.grid.GetCentrePoint();
        maxPan = Math.Max(DungeonManager.instance.grid.GetHeight(), DungeonManager.instance.grid.GetWidth())/1.75f;
    }
}
