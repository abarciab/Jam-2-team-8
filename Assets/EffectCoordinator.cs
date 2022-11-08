using DitzeGames.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCoordinator : MonoBehaviour
{
    public void triggerCardEffect()
    {
        CameraEffects.ShakeOnce();
        AudioManager.instance.PlayGlobal(13);
    }
}
