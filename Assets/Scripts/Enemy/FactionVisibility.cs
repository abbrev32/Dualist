using Mirror;
using UnityEngine;

public class FactionVisibility : NetworkBehaviour
{
    [SerializeField]
    private PlayerFaction.Faction visibleTo;
    public SpriteRenderer sprite;
    public Collider2D hitbox;

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

        Debug.Log($"[FactionVisibility] Local player faction: {localPlayer.faction}, " +
              $"Enemy visibleTo: {visibleTo}");
        bool visible = (localPlayer.faction == visibleTo);
        sprite.enabled = visible;
        hitbox.enabled = visible;
    }
}
