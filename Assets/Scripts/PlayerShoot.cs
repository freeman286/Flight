using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{

    public GameObject bullet;
    public Rigidbody rb;
    public ParticleSystem muzzle;
    public GameObject shootSound;

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

        CmdProjectileShot(transform.position + transform.forward * 10f, transform.rotation, transform.forward * 1000f + rb.velocity, transform.name);
    }

    [Command]
    void CmdProjectileShot(Vector3 _pos, Quaternion _rot, Vector3 _vel, string _playerID) {
        RpcProjectileShot(_pos, _rot, _vel, _playerID);
        RpcDoShootEffect(_pos);
    }

    [ClientRpc]
    void RpcProjectileShot(Vector3 _pos, Quaternion _rot, Vector3 _vel, string _playerID) {
        GameObject _projectile = (GameObject)Instantiate(bullet, _pos, _rot);
        _projectile.GetComponent<Rigidbody>().velocity = _vel;
        _projectile.GetComponent<ProjectileController>().playerID = _playerID;
    }

    [ClientRpc]
    void RpcDoShootEffect(Vector3 _pos) {
        muzzle.GetComponent<ParticleSystem>().Play();
        GameObject _shootSound = (GameObject)Instantiate(
            shootSound,
            _pos,
            new Quaternion(0, 0, 0, 0)
        );
        _shootSound.GetComponent<AudioSource>().Play();
        Destroy(_shootSound, _shootSound.GetComponent<AudioSource>().clip.length);
    }
}
