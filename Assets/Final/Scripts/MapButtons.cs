using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtons : MonoBehaviour
{
    public void goToRoom(string roomName) {
        StartCoroutine(RoomManager.instance.startRoomTransition(roomName));
        JournalManager.instance.CloseJournal();
    }
}
