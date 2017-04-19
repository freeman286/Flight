using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour {

    public TrailRenderer trail;
    public Player player;
    public PlayerMotor motor;
    public Fracturable fracture;
    public float trailTime;


    void Update () {
		if (player.currentHealth > 0 && !fracture.fractured) {
            trail.time = Mathf.Lerp(trail.time, trailTime, 0.9f);
        } else {
            trail.time = Mathf.Lerp(trail.time, 0f, 0.1f);
        }
	}
}
