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

    public Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public Behaviour[] enableOnDeath;

    public Transform trans;

    public Rigidbody rb;

    public Camera cam;

    public PlayerShoot shoot;

    public PlayerController controller;

    public PlayerMotor motor;

    public GameObject hit;

    [SyncVar]
    public int kills = 0;

    [SyncVar]
    public int deaths = 0;

    [SyncVar]
    public int timeSinceSpawned = -1;

    private bool firstSetup = true;

    public GameObject deathParticle;

    public GameObject damagedParticle;

    public GameObject explosion;

    public ParticleSystem flames;

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
            currentHealth = 0;
            Die();
            GameManager.GetPlayer(lastDamage).kills += 1;
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
            _hitSound.transform.SetParent(transform);
            _hitSound.Play();
            Destroy(_hitSound.gameObject, 1f);
        }

        if (timeSinceSpawned > 100) {
            currentHealth -= _amount;
        }

        if (currentHealth >= 50) {
            InvokeRepeating("Damaged", 0f, 0.1f);
        }

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

        for (int i = 0; i < enableOnDeath.Length; i++) {
            enableOnDeath[i].enabled = true;
        }

        CancelInvoke("Damaged");
        InvokeRepeating("Dead", 0f, 0.1f);

        rb.isKinematic = false;

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

        for (int i = 0; i < enableOnDeath.Length; i++) {
            enableOnDeath[i].enabled = false;
        }

        RepairRecursively(transform);

        motor.damage = 1f;
        motor.damageSkew = Vector3.zero;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        flames.Stop();
    }

    void Dead() {
        flames.Play();
        CmdSpawnDeathParticles(transform.position);
    }

    void Damaged() {
        CmdSpawnParticles(transform.position);
    }

    [Command]
    void CmdSpawnDeathParticles(Vector3 _pos) {
        RpcSpawnDeathParticles(_pos);
    }
    
    [ClientRpc]
    void RpcSpawnDeathParticles(Vector3 _pos) {
        GameObject _deathParticle = (GameObject)Instantiate(deathParticle, _pos, Quaternion.identity);
        Destroy(_deathParticle, 5f);
    }

    [Command]
    void CmdSpawnParticles(Vector3 _pos) {
        RpcSpawnParticles(_pos);
    }

    [ClientRpc]
    void RpcSpawnParticles(Vector3 _pos) {
        GameObject _damagedParticle = (GameObject)Instantiate(damagedParticle, _pos, Quaternion.identity);
        Destroy(_damagedParticle, 5f);
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

    public void RepairRecursively(Transform _obj) {
        foreach (Transform child in _obj) {
            RepairRecursively(child.transform);
            if (child.GetComponent<MeshRenderer>() != null) {
                child.GetComponent<MeshRenderer>().enabled = true;
            }
            if (child.GetComponent<BoxCollider>() != null) {
                child.GetComponent<BoxCollider>().enabled = true;
            }
            if (child.GetComponent<Fracturable>() != null) {
                child.GetComponent<Fracturable>().fractured = false;
            }
        }
    }

    public void OnCollisionEnter(Collision _collision) {
        foreach (ContactPoint _contact in _collision.contacts){
            if (_contact.thisCollider.GetComponent<Fracturable>() != null) {
                _contact.thisCollider.GetComponent<Fracturable>().Fracture();
            }
        }
    }
}

