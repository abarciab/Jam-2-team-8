using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtons : MonoBehaviour
{
    public void goToRoom(string roomName) {
        StartCoroutine(RoomManager.instance.startRoomTransition(roomName));
        RoomManager.instance.journalButton.GetComponent<JournalButton>().toggleJournal();
        //RoomManager.instance.journalButton.transform.GetChild(0).gameObject.SetActive(false);
        //JournalManager.instance.CloseJournal();
    }
}
