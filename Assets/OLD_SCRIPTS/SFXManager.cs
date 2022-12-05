using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
  public AudioMovement Player1;
  public AudioSource AudioSourceP1;
  public AudioSource AudioSourceP2;
  public AudioSource AudioSourceCrashes;
  public AudioSource AudioSourceMusic;

  private float P1Volume = 0f;
  private float P2Volume = 0f;

  [Header("SoundEffects")]
  public AudioClip engine;
  public AudioClip crashWall;
  public AudioClip Music;
  public AudioClip MusicDemo;

  public bool isSingleplayer = false;



  void Start(){
    AudioSourceMusic.clip = MusicDemo;
    AudioSourceMusic.volume = 0.5f;
    AudioSourceMusic.Play();

    P1Volume = 0f;
    AudioSourceP1.clip = engine;
    AudioSourceP1.volume = P1Volume;
    AudioSourceP1.Play();

    P2Volume = 0f;
    AudioSourceP2.clip = engine;
    AudioSourceP2.volume = P2Volume;
    AudioSourceP2.Play();
  }
    public void StartGame()
    {
        AudioSourceMusic.Stop();
        AudioSourceMusic.clip = Music;
        AudioSourceMusic.Play();
    }

    public void crashIntoTrack(){
      Debug.Log("Crashed");
      AudioSourceCrashes.clip = crashWall;
      AudioSourceCrashes.Play();
    }

    public void Player1EngineSFX(){
      AudioSourceP1.volume = P1Volume;
      if(Player1.currentSpeed>2f && Player1.currentAmp>-20f){
        if(P1Volume < 0.9f){
          P1Volume += 0.001f;
        }
      }
      else{
        if(P1Volume > 0.1f){
          P1Volume -= 0.001f;
        }
        else if (P1Volume < 0.0999f){
          P1Volume = 0f;
        }
      }
    }
    void Update()
    {
    Player1EngineSFX();

    }
}
