using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Completed

{
    public class QuitToEnding : MonoBehaviour
    {
        Player player;
      
        // Use this for initialization
        public void SkipToEnding()
        {
            //skip to the last scene
            GameManager.instance.GameOver();

            player = GameObject.Find("Player").GetComponent<Player>();
            int oldHighscore = PlayerPrefs.GetInt("highscore", 0);
            //check if the player score is higher then the current highscore
            if (player.score > oldHighscore)
                PlayerPrefs.SetInt("highscore", player.score);

        //load last scene
            SceneManager.LoadScene(2);
        }

        // Update is called once per frame
        public void Menu()
        {
            //set level too 0
            GameManager.level = 0;
           //go back to first scene
            SceneManager.LoadScene(0);

        }

        public void Quit()
        {
           

            //If we are running in a standalone build of the game
            #if UNITY_STANDALONE
                            //Quit the application
                            Application.Quit();
            #endif

                        //If we are running in the editor
            #if UNITY_EDITOR
		                    //Stop playing the scene
		                    UnityEditor.EditorApplication.isPlaying = false;
            #endif

        }

    }
}