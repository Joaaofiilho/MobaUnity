using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static Interactable GetInteractableFromGameObject(GameObject gameObject)
    {
        if (!gameObject)
            return null;

        switch (gameObject.tag)
        {
            case "Minion":
                return gameObject.GetComponent<Minion>() as Interactable;
            case "Map":
                return gameObject.GetComponent<Map>() as Interactable;
            default:
                return null;
        }
    }
}
