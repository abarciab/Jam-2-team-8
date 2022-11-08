using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButton : MonoBehaviour
{
    public void goToRoom(string roomName) {
        if (roomName == RoomManager.instance.currentRoomName) {
            return;
        }
        RoomManager.instance.transitionCoroutine(roomName);
        RoomManager.instance.journalButton.GetComponent<JournalButton>().toggleJournal();
        AudioManager.instance.PlayGlobal(8);
    }

    
}