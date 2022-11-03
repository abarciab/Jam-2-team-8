using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public int backgroundMusicSoundID;
    public int normalMusicSoundID;
    public int intenseMusicSoundID;

    private void Start()
    {
        AudioManager.instance.PlayMusic(backgroundMusicSoundID);
    }
}
