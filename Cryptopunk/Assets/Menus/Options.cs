using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    public static readonly string sfxVolumeKey = "sfxVolume";
    public static readonly string musicVolumeKey = "musicVolume";
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    private AudioSource musicPlayer;
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = FindObjectOfType<MusicManager>().GetComponent<AudioSource>();
        InitializeSoundPrefs();
        SetSliderPositions();
    }

    private void SetSliderPositions()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxVolumeKey);
        musicSlider.value = PlayerPrefs.GetFloat(musicVolumeKey);
    }

    internal static void InitializeSoundPrefs()
    {
        if (!PlayerPrefs.HasKey(sfxVolumeKey))
        {
            PlayerPrefs.SetFloat(sfxVolumeKey, 1f);
        }
        if (!PlayerPrefs.HasKey(musicVolumeKey))
        {
            PlayerPrefs.SetFloat(musicVolumeKey, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat(sfxVolumeKey, sfxSlider.value);
        PlayerPrefs.SetFloat(musicVolumeKey, musicSlider.value);
        if(musicPlayer)
        {
            musicPlayer.volume = musicSlider.value;
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("menu");
    }
}
