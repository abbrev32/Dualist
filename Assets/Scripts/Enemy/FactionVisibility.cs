using Mirror;
using UnityEngine;

public class FactionVisibility : NetworkBehaviour
{
    [SerializeField]
    private PlayerFaction.Faction visibleTo;
    private SpriteRenderer sprite;
    private Collider2D hitbox;

    public override void OnStartClient()
    {

        sprite = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<Collider2D>();

        UpdateVisibility();
    }
    private void Update()
    {
        if (isClient)
        {
            UpdateVisibility();
        }
    }
    void UpdateVisibility()
    {
        var localPlayer = NetworkClient.localPlayer?.GetComponent<PlayerFaction>();
        if (localPlayer == null) return;

        bool visible = (localPlayer.faction == visibleTo);
        sprite.enabled = visible;
        hitbox.enabled = visible;
    }
}
