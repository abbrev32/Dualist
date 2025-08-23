using Mirror;
using Mirror.Discovery;
using System;
using System.Net;
using UnityEngine;

[Serializable]
public class DiscoveryResponse
{
    public long serverId;
    public IPEndPoint endpoint;
    public Uri uri;
}

public class LANDiscovery : NetworkDiscovery
{
    public event Action<ServerResponse> OnServerFoundEvent;

    protected override ServerResponse ProcessRequest(ServerRequest request, IPEndPoint endpoint)
    {
        return new ServerResponse
        {
            uri = transport.ServerUri(),
            EndPoint = endpoint
        };
    }

    protected override void ProcessResponse(ServerResponse response, IPEndPoint endpoint)
    {
        // Ensure EndPoint is set on the client side
        response.EndPoint = endpoint;
        OnServerFoundEvent?.Invoke(response);
    }

}
