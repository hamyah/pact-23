using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonInteractable : Interactable
{
    protected override void Execute() {
        Player.m_PersonInteractableActivated.Invoke(1, "");
        Destroy(gameObject);
    }
}
