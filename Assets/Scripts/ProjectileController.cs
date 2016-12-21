using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ProjectileController : NetworkBehaviour {

    public string playerID;

    private int framesSinceCreated = 0;

    void OnCollisionEnter(Collision _collision) {
        if ((_collision.collider.transform.root.tag == "Player")) {
            _collision.collider.transform.root.GetComponent<Player>().RpcTakeDamage(50, playerID);
        }
    }

    void FixedUpdate() {
        if (framesSinceCreated > 500) {
            Destroy(gameObject);
        }
        framesSinceCreated += 1;
    }
}
