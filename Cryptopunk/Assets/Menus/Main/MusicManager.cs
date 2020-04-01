using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    internal static MusicManager instance;
    // Start is called before the first frame update
    void Start()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Options.InitializeSoundPrefs();
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(Options.musicVolumeKey);
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
