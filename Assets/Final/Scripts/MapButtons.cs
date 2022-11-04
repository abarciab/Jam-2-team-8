using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtons : MonoBehaviour
{
    public void goToRoom(string roomName) {
        RoomManager.instance.transitionCoroutine(roomName);
        RoomManager.instance.journalButton.GetComponent<JournalButton>().toggleJournal();
    }
}
