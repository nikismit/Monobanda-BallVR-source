using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPress : MonoBehaviour
{
void onButtonPress(){
  Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
}
}
