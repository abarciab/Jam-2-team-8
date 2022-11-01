using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Room {
    public string name;
    public Sprite background;
    public List<string> characterNames = new List<string>();
}

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    public int currentRoomNumber;
    public string currentRoomName;
    public List<Room> rooms = new List<Room>();
    public Transform characterHolder;
    public Camera cam;
    private SpriteRenderer sprRenderer;

    private void Awake()
    {
        if(instance == null) {
            instance = this;                    // makes instance a singleton
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
        foreach(Transform child in characterHolder) {
            if(names.Contains(child.GetComponent<CharacterInteract>().characterName)) {
                child.gameObject.SetActive(active);
            }
        }
    }

    private void fitSpriteToScreen(Sprite spr) {
        transform.localScale = new Vector3(Screen.width / (spr.bounds.extents.x * spr.pixelsPerUnit * 2),
                                           Screen.height / (spr.bounds.extents.y * spr.pixelsPerUnit * 2), 
                                           transform.localScale.z);
        transform.position = Vector3.zero;
        return;
    }

    public Room getRoomByName(string roomName) {
        foreach(Room room in rooms) {
            if(room.name == roomName)
                return room;
        }
        Debug.LogError("Room " + roomName + " does not exist");
        return null;
    }

    public void setRoom(string roomName) {
        // disable characters every room
        foreach(Room room in rooms)
            toggleCharacters(room.characterNames, false);

        // get new room, activate it's characters, and set background
        Room newRoom = getRoomByName(roomName);
        toggleCharacters(newRoom.characterNames, true);
        setBackground(newRoom.background);
        fitSpriteToScreen(newRoom.background);
        currentRoomName = newRoom.name;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            setRoom("room2");
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)) {
            setRoom("room1");
        }
        
    }
}
