using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public PlayerMotor motor;
    public Camera cam;

    private float xRot;
    private float yRot;
    private float zRot;

    private int framesWithCursor;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        xRot = -Input.GetAxis("Mouse Y") * Time.deltaTime;
        yRot = Input.GetAxis("Mouse X") * Time.deltaTime;
        zRot = Input.GetAxis("Horizontal") * Time.deltaTime;
        motor.Rotate(xRot, yRot, zRot);

        motor.SpeedUp(Input.GetAxis("Vertical") * Time.deltaTime * 10);

        Vector3 moveCamTo = transform.position - transform.forward * motor.speed * 0.2f + transform.up * 5f;
        cam.transform.position = moveCamTo;
        cam.transform.LookAt(transform.position + transform.forward * 100f);

        //Pressing Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            framesWithCursor = 0;
        }
        else if (Input.GetButtonDown("Fire1") && framesWithCursor > 100)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            framesWithCursor = 0;
        }
        framesWithCursor += 1;
    }
}
