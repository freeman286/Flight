using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    public Behaviour[] ComponentsToDisable;

    Camera sceneCamera;

    public string remoteLayerName;

    public GameObject playerGraphics;

    void Start() {
        if (!isLocalPlayer) {
            DisableComponents();
            AssignRemoteLayer();
        } else {
            sceneCamera = Camera.main;
            if (sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer) {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void DisableComponents() {
        for (int i = 0; i < ComponentsToDisable.Length; i++){
            ComponentsToDisable[i].enabled = false;
        }
    }

    void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void OnDisable() {
        if (sceneCamera != null){
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
