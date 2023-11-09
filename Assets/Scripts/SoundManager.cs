using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource musicSource, sfxSource;
    [SerializeField] private AudioClip[] musicsSound, sfxsSound;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        musicSource.volume = DataManager.Instance.LoadMusicVolume();
        sfxSource.volume = DataManager.Instance.LoadSFXVolume();
    }
    public void ChangeSoundVolume(float volume)
    {
        sfxSource.volume = volume;
        DataManager.Instance.SaveSFXVolume(sfxSource.volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        musicSource.volume = volume;
        DataManager.Instance.SaveMusicVolume(musicSource.volume);
    }

    public void PlaySfx(SfxType type)
    {
        switch (type)
        {
            case SfxType.ButtonClick:
                sfxSource.PlayOneShot(sfxsSound[0]);
                break;
            case SfxType.TetrominoClick:
                sfxSource.PlayOneShot(sfxsSound[1]);
                break;
            case SfxType.OnBoard:
                sfxSource.PlayOneShot(sfxsSound[2]);
                break;
            case SfxType.GameOver:
                sfxSource.PlayOneShot(sfxsSound[3]);
                break;
            case SfxType.Complete:
                sfxSource.PlayOneShot(sfxsSound[4]);
                break;
        }
    }
    public void PlayMusic(MusicType type)
    {
        switch (type)
        {
            case MusicType.MainMenu:
                musicSource.clip = musicsSound[0];
                musicSource.Play();
                break;
            case MusicType.GamePlay:
                musicSource.clip = musicsSound[1];
                musicSource.Play();
                break;
        }
    }
}
public enum SfxType
{
    ButtonClick, TetrominoClick, OnBoard, GameOver, Complete
}
public enum MusicType
{
    MainMenu, GamePlay
}