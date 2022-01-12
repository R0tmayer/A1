using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject menuBlock;

    [SerializeField] private GameObject musicOn;
    [SerializeField] private GameObject musicOff;

    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject soundOff;

    [SerializeField] private GameObject BuildButtons;

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }    
    
    public void Pause() {
        pauseButton.SetActive(false);
        menuBlock.SetActive(true);
        BuildButtons.SetActive(false);
        Time.timeScale = 0;
    }

    public void Continue() {
        pauseButton.SetActive(true);
        menuBlock.SetActive(false);
        BuildButtons.SetActive(true);
        Time.timeScale = 1;
    }
    

    public void SoundOn() {
        soundOn.SetActive(true);
        soundOff.SetActive(false);
    }
        

    public void SoundOff() {
        soundOn.SetActive(false);
        soundOff.SetActive(true);
    }        

    public void MusicOn() {
        musicOn.SetActive(true);
        musicOff.SetActive(false);
    }

    public void MusicOff() {
        musicOn.SetActive(false);
        musicOff.SetActive(true);
    }

    public void Info() {
        Application.OpenURL("https://www.a1.by/");
    }



}