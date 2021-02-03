using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Champion))]
public class Player : MonoBehaviour
{
    [HideInInspector]
    public Champion champion;

    [SerializeField] private Camera playerCamera;

    private void Awake()
    {
        champion = GetComponent<Champion>();
        champion.playerCamera = playerCamera;
    }

    public void OnActionInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        champion.Action(champion.gameObject);
    }

    public void OnStopAllInput(InputAction.CallbackContext context)
    {
        champion.StopAll();
    }
}