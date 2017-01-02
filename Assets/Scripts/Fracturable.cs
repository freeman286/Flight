using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class Fracturable : MonoBehaviour {

    public GameObject shards;

    private MeshRenderer mesh;
    private BoxCollider box;

    void Start() {
        mesh = GetComponent<MeshRenderer>();
        box = GetComponent<BoxCollider>();
    }

    public void Fracture() {
        mesh.enabled = false;
        box.enabled = false;
        GameObject _shards = (GameObject)Instantiate(shards, transform.position, transform.rotation);
        _shards.GetComponent<Rigidbody>().velocity = transform.root.forward * transform.root.GetComponent<PlayerMotor>().speed;
        _shards.transform.localScale = transform.localScale;
    }

}
