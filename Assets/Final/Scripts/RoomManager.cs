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
        print(roomName);
        Room newRoom = getRoomByName(roomName);
        //Room currentRoom = rooms[currentRoomNumber];

        foreach(Room room in rooms)
            toggleCharacters(room.characterNames, false);

        //toggleCharacters(currentRoom.characterNames, false);
        toggleCharacters(newRoom.characterNames, true);
        setBackground(newRoom.background);
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
