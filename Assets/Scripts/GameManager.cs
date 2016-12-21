using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;

    public MatchSettings matchSettings;


    void Awake() {
        if (instance != null)
        {
            Debug.Log("More than one GameManager in scene");
        }
        else {
            instance = this;
        }
    }

    private const string PLAYER_ID_PREFIX = "Player ";

    public static Dictionary<string, Player> players = new Dictionary<string, Player>();
    public static Dictionary<string, string> names = new Dictionary<string, string>();

    public static void RegisterPlayer(string _netID, Player _player) {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
        names.Add(_playerID, _playerID);
    }

    public static void UnRegisterPlayer(string _playerID) {
        players.Remove(_playerID);
        names.Remove(_playerID);
        Destroy(GameObject.Find(_playerID + "UI"));
    }

    public static string[] Players() {
        return players.Keys.ToArray();
    }

    public static Player GetPlayer(string _playerID) {
        return players[_playerID];
    }

    public static int PlayerCount() {
        return players.Keys.Count;
    }


}

