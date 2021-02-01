using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    /// <summary>
    /// Method called when a select action occours on a Interactable GameObject.
    /// </summary>
    /// <param name="actor">The GameObject who interacted with it.</param>
    void OnSelect(GameObject actor);

    /// <summary>
    /// Method called when an act action occours on a Interactable GameObject.
    /// </summary>
    /// <param name="actor">The GameObject who interacted with it.</param>
    void OnAction(GameObject actor);
}