using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public PlayerMotor motor;
    public Camera cam;

    private float xRot;
    private float yRot;
    private float zRot;

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
    }
}
