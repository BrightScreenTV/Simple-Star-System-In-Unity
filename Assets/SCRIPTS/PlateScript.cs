using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateScript : MonoBehaviour
{
    public bool onScreen = false;

    private bool _wasOnScreen = false;

    public GameObject player; // this is the reference so we can work out the relative distance

    Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    void debugFrame(Rect aRect, Color colour) {
        Debug.DrawLine(new Vector3(aRect.min.x, aRect.min.y, 0.0f), new Vector3(aRect.max.x, aRect.min.y, 0.0f), colour);
        Debug.DrawLine(new Vector3(aRect.max.x, aRect.min.y, 0.0f), new Vector3(aRect.max.x, aRect.max.y, 0.0f), colour);
        Debug.DrawLine(new Vector3(aRect.max.x, aRect.max.y, 0.0f), new Vector3(aRect.min.x, aRect.max.y, 0.0f), colour);
        Debug.DrawLine(new Vector3(aRect.min.x, aRect.max.y, 0.0f), new Vector3(aRect.min.x, aRect.min.y, 0.0f), colour);
    }

    /// Returns `true` if the plate has become invisible since this method was last called
    bool goneInvisible() {
        if (_wasOnScreen && !onScreen) {
            _wasOnScreen = false;
            return true;
        }
        return false;
    }

    Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
        }

/**
LEGACY CODE
    // Update is called once per frame
    void Update()
    {
        SpriteRenderer thisRenderer = GetComponent<SpriteRenderer>();
        if (thisRenderer == null) { return; }
        Vector3 lowerLeftBound = mainCam.ViewportToWorldPoint(new Vector3(0.0f,0.0f,0.0f));
        Vector3 upperRightBound = mainCam.ViewportToWorldPoint(new Vector3(1.0f,1.0f,0.0f));
        Rect camRect = new Rect(lowerLeftBound.x, lowerLeftBound.y, upperRightBound.x - lowerLeftBound.x, upperRightBound.y - lowerLeftBound.y);

        //This is in world space
        float spriteWidth = thisRenderer.sprite.bounds.max.x - thisRenderer.sprite.bounds.min.x;
        float spriteHeight = thisRenderer.sprite.bounds.max.y - thisRenderer.sprite.bounds.min.y;

        Rect thisRect = new Rect(transform.position.x - (spriteWidth / 2), transform.position.y - (spriteHeight / 2), spriteWidth, spriteHeight);

        Vector3 topLeft = new Vector3(thisRect.xMin, thisRect.yMin, transform.position.z);
        Vector3 bottomLeft = new Vector3(thisRect.xMin, thisRect.yMax, transform.position.z);
        Vector3 topRight = new Vector3(thisRect.xMax, thisRect.yMin, transform.position.z);
        Vector3 bottomRight = new Vector3(thisRect.xMax, thisRect.yMax, transform.position.z);

        Vector3 screenTopLeft = mainCam.WorldToScreenPoint(topLeft);
        Vector3 screenBottomLeft = mainCam.WorldToScreenPoint(bottomLeft);
        Vector3 screenTopRight = mainCam.WorldToScreenPoint(topRight);
        Vector3 screenBottomRight = mainCam.WorldToScreenPoint(bottomRight);

        Rect screenRect = new Rect(topLeft.x, topLeft.y, topRight.x - topLeft.x, bottomLeft.y - topLeft.y);
        debugFrame(screenRect, Color.red);

        if (camRect.Overlaps(thisRect, true)) {
            onScreen = true;
        } else {
            onScreen = false;
        }
    }
    */
}
