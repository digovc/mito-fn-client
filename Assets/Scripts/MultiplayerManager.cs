using Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public GameObject player;
    private readonly Dictionary<byte, GameObject> _players = new Dictionary<byte, GameObject>();
    private PacketBroadcaster _broadcaster;
    private UdpClient _client;
    private Vector3 _lastPosition;
    private float _lastTick = 0;
    private byte _playerGlobalID;
    private string _uid;

    public void Login()
    {
        _uid = Guid.NewGuid().ToString();

        var packet = new LoginRequest
        {
            UID = _uid,
        };

        _client.Send(packet);
    }

    public void Start()
    {
        StartPlayer();
        StartClient();
        StartBroadcaster();
    }

    public void Update()
    {
        _lastTick += Time.deltaTime;

        if (_lastTick < 0.065)
        {
            _lastTick += Time.deltaTime;
            return;
        }

        SyncPosition(player.transform.position);
    }

    private void Login(object sender, LoginResponse packet)
    {
        _playerGlobalID = packet.GlobalID;
    }

    private void Move(object sender, Move packet)
    {
        var player = _players.First(x => x.Key == packet.GlobalID);
        var position = new Vector3(packet.X, packet.Y, packet.Z);
        MovePlayer(player.Value, position);
    }

    private void MovePlayer(GameObject player, Vector3 position)
    {
        // O player deve se mover para uma nova posição.
        throw new NotImplementedException();
    }

    private void PlayerJoined(object sender, PlayerJoined packet)
    {
        var player = SpawnPlayer();
        _players[packet.GlobalID] = player;
    }

    private GameObject SpawnPlayer()
    {
        // O player deve ser adicionado para o mundo e retornar a instância do gameObject dele.
        throw new NotImplementedException();
    }

    private void StartBroadcaster()
    {
        _broadcaster = gameObject.AddComponent<PacketBroadcaster>();
        _broadcaster.OnLogin += Login;
        _broadcaster.OnMove += Move;
        _broadcaster.OnPlayerJoined += PlayerJoined;
    }

    private void StartClient()
    {
        _client = gameObject.AddComponent<UdpClient>();
    }

    private void StartPlayer()
    {
        if (!player)
        {
            throw new NullReferenceException("Player must be setted.");
        }
    }

    private void SyncPosition(Vector3 position)
    {
        if (position.Equals(_lastPosition))
        {
            return;
        }

        _lastPosition = position;

        var packet = new Move
        {
            GlobalID = _playerGlobalID,
            X = position.x,
            Y = position.y,
            Z = position.z,
        };

        _client.Send(packet);
    }
}