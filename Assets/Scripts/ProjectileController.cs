using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ProjectileController : NetworkBehaviour {

    public string playerID;

    public GameObject hitParticle;

    private int framesSinceCreated = 0;

    void OnCollisionEnter(Collision _collision) {
        GameObject _hitParticle = (GameObject)Instantiate(hitParticle, _collision.contacts[0].point, Quaternion.FromToRotation(Vector3.up, _collision.contacts[0].normal));


        if ((_collision.collider.transform.root.tag == "Player")) {
            _collision.collider.transform.root.GetComponent<Player>().RpcTakeDamage(50, playerID);
            if (_collision.collider.GetComponent<Fracturable>() != null) {
                _collision.collider.GetComponent<Fracturable>().Fracture();
            }
        }

        Destroy(gameObject);
        Destroy(_hitParticle, 10f);
    }

    void FixedUpdate() {
        if (framesSinceCreated > 500) {
            Destroy(gameObject);
        }
        framesSinceCreated += 1;
    }
}
