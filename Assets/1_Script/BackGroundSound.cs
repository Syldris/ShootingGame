using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BackGroundSound : MonoBehaviour
{
    public Sprite[] sprite;
    public Button BGIconButton;

    public bool isBGSound;
    public ObjectManager objectmanager;

    public void BGSoundOnOff()
    {
        if(isBGSound)
        {
            isBGSound = false;
            objectmanager.backgroundSound.Stop();
            BGIconButton.image.sprite = sprite[0]; 
        }
        else
        {
            isBGSound = true;
            objectmanager.backgroundSound.Play();
            BGIconButton.image.sprite = sprite[1];
        }
    }
}
