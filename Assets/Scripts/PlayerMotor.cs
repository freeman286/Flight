using UnityEngine;
using System.Collections;

public class PlayerMotor : MonoBehaviour {
    public Rigidbody rb;

    public float speed;
    public float _xSen;
    public float _ySen;
    public float _zSen;

    public float maxSpeed;
    public float minSpeed;

    public float acceleration;

    public float damage = 1f;
    public Quaternion damageSkew;

    public void Rotate (float _x, float _y, float _z) {
        transform.Rotate(Mathf.Clamp(_x * _xSen, -0.5f, 0.5f), Mathf.Clamp(_y * _ySen, -0.5f, 0.5f), Mathf.Clamp(_z * _zSen, -0.5f, 0.5f));
    }

    void Update () {
        transform.position += damageSkew * (transform.forward * speed * Time.deltaTime);
        speed -= transform.forward.y * acceleration * Time.deltaTime;

        speed = Mathf.Clamp(speed, minSpeed, maxSpeed * damage);
    }

    public void SpeedUp (float _amount) {
        speed += _amount;
    }
}
