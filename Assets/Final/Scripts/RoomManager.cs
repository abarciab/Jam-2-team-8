using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Room {
    public string name;
    public Sprite background;
    public List<string> characterNames = new List<string>();
    public List<string> evidenceNames = new List<string>();
}

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    public int currentRoomNumber;
    public string currentRoomName;
    public List<Room> rooms = new List<Room>();
    public Transform characterHolder;
    public Transform fade;
    public Transform evidencePickups;
    public Camera cam;
    public bool transitioning = false;
    public bool setNewRoom = false;
    public Transform journalButton;
    private SpriteRenderer sprRenderer;

    private void Awake()
    {
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            fade.gameObject.SetActive(true);
            sprRenderer = GetComponent<SpriteRenderer>();
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }

    private void Start() {
        setRoom(currentRoomName);
    }

    private void setBackground(Sprite newBackground) {
        sprRenderer.sprite = newBackground;
    }

    private void toggleCharacters(List<string> names, bool active) {
        if(names.Count <= 0)
            return;

        foreach(Transform child in characterHolder) {
            if(names.Contains(child.GetComponent<CharacterInteract>().characterName)) {
                child.gameObject.SetActive(active);
            }
        }
    }

    private void toggleEvidencePickups(List<string> evidenceNames, bool active) {
        if(evidenceNames.Count <= 0)
            return;

        // loop through all evidence
        foreach(Transform child in evidencePickups) {
            EvidencePickup pickup = child.GetComponent<EvidencePickup>();
            
            // if activating evidence
            if(active == true){
                // activate if pickup should be in the room and is not already collected
                if(evidenceNames.Contains(pickup.evidenceName) && !pickup.collected && 
                   pickup.allowedRealities.Contains(RealityManager.instance.currentReality.name))
                    child.gameObject.SetActive(true);
                print(RealityManager.instance.currentReality.name);
            }
            // if deactivating evidence
            else {
                // deactivate if pickup is in room
                if(evidenceNames.Contains(pickup.evidenceName))
                    child.gameObject.SetActive(false);
            }
        }
    }

    private void fitSpriteToScreen(Sprite spr) {
        transform.localScale = new Vector3(Screen.width / (spr.bounds.extents.x * spr.pixelsPerUnit * 2),
                                           Screen.height / (spr.bounds.extents.y * spr.pixelsPerUnit * 2), 
                                           transform.localScale.z);
        transform.position = Vector3.zero;
    }

    private void setRoom(string roomName) {
        // disable characters every room
        foreach(Room room in rooms) {
            toggleCharacters(room.characterNames, false);
            toggleEvidencePickups(room.evidenceNames, false);
        }

        // get new room, activate it's characters and evidence, and set background
        Room newRoom = getRoomByName(roomName);
        toggleCharacters(newRoom.characterNames, true);
        toggleEvidencePickups(newRoom.evidenceNames, true);
        setBackground(newRoom.background);
        if (newRoom.background == null) { return; }
        fitSpriteToScreen(newRoom.background);
        currentRoomName = newRoom.name;
    }

    public void resetJournalButton() {
        journalButton.GetComponent<JournalButton>().toggleJournal();
    }

    public Room getRoomByName(string roomName) {
        foreach(Room room in rooms) {
            if(room.name == roomName)
                return room;
        }
        Debug.LogError("Room " + roomName + " does not exist");
        return null;
    }

    public IEnumerator startRoomTransition(string roomName) {
        if(roomName == currentRoomName || transitioning)
            yield break;

        // begin fade transition
        UIManager.instance.hideDialogueUI(false);
        fade.gameObject.SetActive(true);
        fade.GetComponent<Animator>().SetTrigger("fade");

        // when fade is done, set new room
        yield return new WaitUntil(() => setNewRoom == true);
        setNewRoom = false;
        setRoom(roomName);
    }

    public void goOutside() {
        setRoom("outside");
    }
    public void goHallway() {
        setRoom("hallway");
    }
    public void goBallroom() {
        setRoom("ballroom");
    }
    public void goOffice() {
        setRoom("office");
    }
    public void goSecurity() {
        setRoom("security");
    }

    private void Update() {
        if(!transitioning) {
            if(Input.GetKeyDown(KeyCode.Alpha1)) {
                StartCoroutine(startRoomTransition("outside"));
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)) {
                StartCoroutine(startRoomTransition("hallway"));
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)) {
                StartCoroutine(startRoomTransition("ballroom"));
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)) {
                StartCoroutine(startRoomTransition("office"));
            }
            if(Input.GetKeyDown(KeyCode.Alpha5)) {
                StartCoroutine(startRoomTransition("security"));
            }
        }
        
    }
}
