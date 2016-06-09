using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

 






    public float panSpeed = 2.0f;       // Speed of the camera when being panned


    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isMoving;     // Is the camera being panned?

    //
    // UPDATE
    //

    void Update()
    {

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize>1)
        {
            Camera.main.orthographicSize--;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize <=8)
        {
            Camera.main.orthographicSize++;
        }

        //check when button is released
        if (!Input.GetMouseButton(1)) isMoving = false;

        // Get the right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isMoving = true;
        }
        



        // Move the camera on it's XY plane
        if (isMoving)
        {


            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            transform.Translate(move, Space.Self);
        }



    }
}
