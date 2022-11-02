using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeAnim : MonoBehaviour
{
    private void startSetRoom() {
        // signal room manager to set the new room
        RoomManager.instance.setNewRoom = true;
    }

    private void doneTransitioning() {
        // signal room manager that transition is finished and disable gameobject
        RoomManager.instance.transitioning = false;
        this.gameObject.SetActive(false);
    }
}
