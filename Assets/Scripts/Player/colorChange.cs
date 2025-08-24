using UnityEngine;
using Mirror;

public class HostClientColor : NetworkBehaviour
{
    public Color hostColor = Color.white;
    public Color clientColor = Color.black;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (isServer && isClient) // Host (server + client)
        {
            sr.color = hostColor;
        }
        else if (isClient && !isServer) // Pure client
        {
            sr.color = clientColor;
        }
    }
}
