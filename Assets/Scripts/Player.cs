using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Champion))]
public class Player : MonoBehaviour
{
    private Champion _champion;

    [SerializeField] private Camera playerCamera;

    private void Awake()
    {
        _champion = GetComponent<Champion>();
        _champion.playerCamera = playerCamera;
    }

    public void OnActionInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        _champion.Action(_champion.gameObject);
    }

    public void OnStopAllInput(InputAction.CallbackContext context)
    {
        _champion.StopAll();
    }
}