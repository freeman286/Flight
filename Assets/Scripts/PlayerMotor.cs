using UnityEngine;
using System.Collections;

public class PlayerMotor : MonoBehaviour {
    public Rigidbody rb;

    public float speed;
    public float _xSen;
    public float _ySen;
    public float _zSen;

    public void Rotate (float _x, float _y, float _z) {
        transform.Rotate(Mathf.Clamp(_x * _xSen, -1f, 1f), Mathf.Clamp(_y * _ySen, -1, 1f), Mathf.Clamp(_z * _zSen, -1f, 1f));
    }

    void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
        speed -= transform.forward.y * 50f * Time.deltaTime;

        speed = Mathf.Clamp(speed, 35, 200);
    }
}
