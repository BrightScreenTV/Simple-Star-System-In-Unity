using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public float turnSpeed = 40.0f; //doesn't matter what you set here - it's always zero to begin with in the inspector

    float velocity = 0.0f;

    public float acceleration;

    public float maxVelocity;

    public bool lockShipDirection;

    public bool wrapXPosition;
    public float xBoundary;
    private float _XBoundary;

    public bool wrapYPosition;
    public float yBoundary;
    private float _YBoundary;

    public StarFieldScript starField;

    float t = 0; //used in Lerp for acceleration


    float targetVelocity = 0.0f; // this is the value the velocity should head towards when 's' is pressed

    public Camera satelliteCamera;

    private Camera mainCam;

    void debugFrame(Rect aRect, Color colour) {
        Debug.DrawLine(new Vector3(aRect.min.x, aRect.min.y, 0.0f), new Vector3(aRect.max.x, aRect.min.y, 0.0f), colour);
        Debug.DrawLine(new Vector3(aRect.max.x, aRect.min.y, 0.0f), new Vector3(aRect.max.x, aRect.max.y, 0.0f), colour);
        Debug.DrawLine(new Vector3(aRect.max.x, aRect.max.y, 0.0f), new Vector3(aRect.min.x, aRect.max.y, 0.0f), colour);
        Debug.DrawLine(new Vector3(aRect.min.x, aRect.max.y, 0.0f), new Vector3(aRect.min.x, aRect.min.y, 0.0f), colour);
    }

    void Start()
    {
        _YBoundary = Mathf.Max(100.0f, yBoundary);
        _XBoundary = Mathf.Max(100.0f, xBoundary);

        mainCam = Camera.main;

        targetVelocity = velocity;
    }

    void Update()
    {
        Vector3 negRotationVector = Vector3.forward * -turnSpeed * Time.deltaTime;
        Vector3 posRotationVector = Vector3.forward * turnSpeed * Time.deltaTime;

        if (Input.GetKey("right"))
        {
            transform.Rotate(negRotationVector, Space.Self);
            satelliteCamera.transform.Rotate(negRotationVector, Space.Self);
            mainCam.transform.Rotate(posRotationVector, Space.Self);
        }
        if (Input.GetKey("left"))
        {
            transform.Rotate(posRotationVector, Space.Self);
            satelliteCamera.transform.Rotate(posRotationVector, Space.Self);
            mainCam.transform.Rotate(negRotationVector, Space.Self);
        }
        if (Input.GetKey("q"))
        {
            targetVelocity = velocity + acceleration;
            t = 0;
        }
        if (Input.GetKey("a"))
        {
            targetVelocity = velocity - acceleration;
            t = 0;
        }
        if (Input.GetKey("s"))
        {
            targetVelocity = 0.0f;
            t = 0;
        }

        if (Input.GetKey("h")) {
            transform.position = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f), 0.0f);
            
        }

        velocity = Mathf.Lerp(velocity, targetVelocity, t);
        t += 0.5f * Time.deltaTime;

        velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);

        if (starField != null) {
            Quaternion fieldRotate = transform.rotation;
            float zRot = transform.rotation.x / 360;
            starField.transform.RotateAround(starField.transform.position, new Vector3(-transform.rotation.z,0,0), -velocity * 10 * Time.deltaTime);
        }

        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (wrapXPosition) {
            pos.x = Mathf.Repeat(pos.x + _XBoundary, _XBoundary * 2) - _XBoundary;
        }
        if (wrapYPosition) {
            pos.y = Mathf.Repeat(pos.y + _YBoundary, _YBoundary * 2) - _YBoundary;
        }

        transform.position = pos;
        transform.Translate(Vector2.up * velocity, Space.Self);

        satelliteCamera.transform.Translate(Vector2.up * velocity, Space.Self);
    }
}