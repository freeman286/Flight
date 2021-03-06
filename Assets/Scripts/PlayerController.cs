﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public PlayerMotor motor;
    public Camera cam;

    private float xRot;
    private float yRot;
    private float zRot;

    private bool inverted = false;

    private int framesWithCursor;

    private Vector3 originPosition;
    private Quaternion originRotation;
    private Vector3 shakePosition;
    private Quaternion shakeRotation;

    void Update () {

        xRot = -Input.GetAxis("Mouse Y") * Time.deltaTime;
        yRot = Input.GetAxis("Mouse X") * Time.deltaTime;
        zRot = Input.GetAxis("Horizontal") * Time.deltaTime;

        if (inverted) {
            xRot *= -1;
        }

        motor.Rotate(xRot, yRot, zRot);

        motor.SpeedUp(Input.GetAxis("Vertical") * Time.deltaTime * 10);

        Vector3 moveCamTo = transform.position - transform.forward * motor.speed * 0.2f + transform.up * 5f;
        cam.transform.position = moveCamTo;
        cam.transform.LookAt(transform.position + transform.forward * 100f);
        cam.transform.localRotation = Quaternion.Euler(cam.transform.rotation.x, cam.transform.rotation.y, Mathf.Lerp(0, zRot * 200f, 0.1f));

        cam.gameObject.GetComponent<CameraShake>().ShakeCamera(Mathf.Pow(motor.speed * (1f / Mathf.Pow(motor.damage, 5)), 4) * 0.00001f, 0.001f * (1f / Mathf.Pow(motor.damage, 3)));

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

        if (Input.GetKeyDown("f")) {
            inverted = !inverted;
        }

        framesWithCursor += 1;
    }
}
