using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnClouds : MonoBehaviour {

    public GameObject cloud;

    public int maxClouds;
    private int currentClouds;
    private int x;
    private int y;
    private int z;
    private int clump;
    private int rot;

    void Start() {
        Random.InitState(System.DateTime.Now.Day * System.DateTime.Now.Month * System.DateTime.Now.Year);

        while (currentClouds <= maxClouds) {
            x = Random.Range(-1600, 1600);
            y = Random.Range(320, 640);
            z = Random.Range(-1600, 1600);

            clump = Random.Range(0, 64);
            for (int i = 0; i < clump; i++) {
                GameObject _cloud = (GameObject)Instantiate(cloud, new Vector3(x, y, z), Quaternion.identity);
                x += Random.Range(-64, 64);
                y += Random.Range(-64, 64);
                z += Random.Range(-64, 64);
            }

            currentClouds += 1;
        }

    }

}
