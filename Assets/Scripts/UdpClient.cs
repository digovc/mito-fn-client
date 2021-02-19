using LiteNetLib;
using LiteNetLib.Utils;
using Packets;
using System;
using UnityEngine;

public class UdpClient : MonoBehaviour
{
    private PacketBroadcaster _broadcaster;
    private NetManager _client;
    private float _lastTick = 0;
    private EventBasedNetListener _listener;
    private MultiplayerManager _mpManager;
    private NetPacketProcessor _processor;

    public void OnDestroy()
    {
        _client.Stop();
    }

    public void Start()
    {
        StartMultiplayerManager();
        StartBroadcaster();
        StartProcessor();
        StartListener();
        StartClient();
    }

    public void Update()
    {
        _lastTick += Time.deltaTime;

        if (_lastTick < 0.065)
        {
            _lastTick += Time.deltaTime;
            return;
        }

        _client.PollEvents();
        _lastTick = 0;
    }

    internal void Send<T>(T packet) where T : class, new()
    {
        var data = _processor.Write(packet);
        var peer = _client.FirstPeer;
        peer.Send(data, DeliveryMethod.ReliableUnordered);
    }

    private void NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        _processor.ReadAllPackets(reader, peer);
    }

    private void PeerConnectedEvent(NetPeer peer)
    {
        _mpManager.Login();
    }

    private void StartBroadcaster()
    {
        _broadcaster = gameObject.GetComponent<PacketBroadcaster>();
    }

    private void StartClient()
    {
        _client = new NetManager(_listener);
        _client.Start();
        _client.Connect("localhost", 9876, "Bbse2vgBTP4t2OMAPumypfV9957FtfMgKRfLk2Mkyw1ChxiAzo");
    }

    private void StartListener()
    {
        _listener = new EventBasedNetListener();
        _listener.NetworkReceiveEvent += NetworkReceiveEvent;
        _listener.PeerConnectedEvent += PeerConnectedEvent;
    }

    private void StartMultiplayerManager()
    {
        _mpManager = gameObject.GetComponent<MultiplayerManager>();
    }

    private void StartProcessor()
    {
        _processor = new NetPacketProcessor();

        _processor.SubscribeReusable<LoginResponse, NetPeer>(_broadcaster.Login);
        _processor.SubscribeReusable<PlayerJoined, NetPeer>(_broadcaster.PlayerJoined);
        _processor.SubscribeReusable<Move, NetPeer>(_broadcaster.Move);
    }
}