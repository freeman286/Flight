using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{

    public GameObject bullet;
    public Rigidbody rb;
    public ParticleSystem muzzle;

    // Use this for initialization
    void Start() {
    }

    void FixedUpdate() {
        if (Input.GetButtonDown("Fire1")) {
            InvokeRepeating("Shoot", 0f, 0.1f);
        }
        if (Input.GetButtonUp("Fire1")) {
            CancelInvoke("Shoot");
        }
    }

    void Shoot() {
        if (!isLocalPlayer)
            return;

        CmdProjectileShot(transform.position + transform.forward * 10f, transform.rotation, transform.forward * 1000f + rb.velocity);
    }

    [Command]
    void CmdProjectileShot(Vector3 _pos, Quaternion _rot, Vector3 _vel) {
        RpcProjectileShot(_pos, _rot, _vel);
        RpcDoShootEffect();
    }

    [ClientRpc]
    void RpcProjectileShot(Vector3 _pos, Quaternion _rot, Vector3 _vel) {
        GameObject _projectile = (GameObject)Instantiate(bullet, _pos, _rot);
        _projectile.GetComponent<Rigidbody>().velocity = _vel;
    }

    [ClientRpc]
    void RpcDoShootEffect() {
        muzzle.GetComponent<ParticleSystem>().Play();
    }
}
