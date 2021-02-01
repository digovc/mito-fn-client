using LiteNetLib;
using Packets;
using System;
using UnityEngine;

public class PacketBroadcaster : MonoBehaviour
{
    internal void Login(LoginResponse packet, NetPeer peer)
    {
        OnLogin?.Invoke(peer, packet);
    }

    internal void Move(Move packet, NetPeer peer)
    {
        OnMove?.Invoke(peer, packet);
    }

    internal void PlayerJoined(PlayerJoined packet, NetPeer peer)
    {
        OnPlayerJoined?.Invoke(peer, packet);
    }

    public event EventHandler<LoginResponse> OnLogin;

    public event EventHandler<Move> OnMove;

    public event EventHandler<PlayerJoined> OnPlayerJoined;
}