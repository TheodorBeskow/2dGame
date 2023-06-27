using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
   public void EnterSplitScreen() {
        SceneManager.LoadScene(1);
   }
   public void EnterMultiPlayer() {
        SceneManager.LoadScene(3);
   }
    public void EnterSinglePlayer() {
        SceneManager.LoadScene(2);
   }
    public void QuitGame() {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
