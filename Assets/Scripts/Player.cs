using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;

public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    public bool isDamaged {
        get { return currentHealth != maxHealth; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    public int currentHealth;

    [SyncVar]
    public string lastDamage;

    [SyncVar]
    private int healthRegen = 0;

    public Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public Transform trans;

    public Rigidbody rb;

    public Camera cam;

    public PlayerShoot shoot;

    public PlayerController controller;

    public GameObject hit;

    [SyncVar]
    public int kills = 0;

    [SyncVar]
    public int deaths = 0;

    [SyncVar]
    public int timeSinceSpawned = -1;

    private bool firstSetup = true;

    public GameObject deathParticle;

    public GameObject explosion;

    public void PlayerSetup() {

        CmdBroadCastNewPlayerSetup();

        SetDefaults();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup() {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients() {
        if (firstSetup) {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++) {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            foreach (var _playerID in GameManager.players.Keys) {
                GameManager.GetPlayer(_playerID).CmdBroadCastNewPlayerSetup();
            }

            firstSetup = false;
        }
        else {
            for (int i = 0; i < disableOnDeath.Length; i++) {
                disableOnDeath[i].enabled = true;
            }
        }

        SetDefaults();
    }

    void Update() {
        int _bound = GameManager.instance.matchSettings.bounds;
        if ((rb.position.y < -10 || rb.position.x > _bound || rb.position.x < -_bound || rb.position.z > _bound || rb.position.z < -_bound) && !isDead) {
            Die();
            GameManager.GetPlayer(lastDamage).kills += 1;
        }
        healthRegen += 1;
        if (healthRegen > 1000 && currentHealth < maxHealth) {
            currentHealth += 1;
        }

        timeSinceSpawned += 1;
    }

    [ClientRpc]
    public void RpcTakeDamage(int _amount, string _shooterID) {
        if (isDead || _amount < 0)
            return;

        if (_shooterID != transform.name) {
            lastDamage = _shooterID;
        }

        if (isLocalPlayer && _amount != 0) {
            AudioSource _hitSound = (AudioSource)Instantiate(
                hit.GetComponent<AudioSource>(),
                cam.transform.position,
                new Quaternion(0, 0, 0, 0)
            );
            _hitSound.Play();
            Destroy(_hitSound.gameObject, 1f);
        }

        if (timeSinceSpawned > 100) {
            currentHealth -= _amount;
        }

        healthRegen = 0;

        if (currentHealth <= 0 && lastDamage != transform.name) {
            Die();
            GameManager.GetPlayer(lastDamage).kills += 1;
        }
    }

    private void Die() {

        deaths += 1;

        isDead = true;

        shoot.CancelInvoke("Shoot");

        for (int i = 0; i < disableOnDeath.Length; i++) {
            disableOnDeath[i].enabled = false;
        }

        InvokeRepeating("Dead", 0f, 0.1f);

        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn() {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        CancelInvoke("Dead");

        CmdBroadCastNewPlayerSetup();

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        RpcSpawnExplosion(transform.position);
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        SetDefaults();
    }

    public void SetDefaults() {
        isDead = false;

        if (timeSinceSpawned > 0) {
            timeSinceSpawned = 0;
        }

        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++) {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        rb.velocity = Vector3.zero;
    }

    void Dead() {
        CmdSpawnParticles(transform.position);
    }

    [Command]
    void CmdSpawnParticles(Vector3 _pos) {
        RpcSpawnParticles(_pos);
    }
    
    [ClientRpc]
    void RpcSpawnParticles(Vector3 _pos) {
        GameObject _deathParticle = (GameObject)Instantiate(deathParticle, _pos, Quaternion.identity);
        Destroy(_deathParticle, 5f);
    }

    [Command]
    void CmdSpawnExplosion(Vector3 _pos) {
        RpcSpawnExplosion(_pos);
    }

    [ClientRpc]
    void RpcSpawnExplosion(Vector3 _pos) {
        GameObject _explosion = (GameObject)Instantiate(explosion, _pos, Quaternion.identity);
        _explosion.GetComponent<AudioSource>().Play();
        Destroy(_explosion, 5f);
    }
}

