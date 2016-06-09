using UnityEngine;
using System.Collections;
namespace Completed

{ 
public class QuitApplication : MonoBehaviour {
        Player player;
	 public void Quit()
	{
           /* if (GameManager.instance)
            {
                player = GameObject.Find("Player").GetComponent<Player>();
                int oldHighscore = PlayerPrefs.GetInt("highscore", 0);

                //Debug.Log("TEST E " + oldHighscore);
                if (player.score > oldHighscore)
                    PlayerPrefs.SetInt("highscore", player.score);
            }*/
          
        //Debug.Log("TEST E " +oldHighscore);
        //Call the GameOver function of GameManager.
       
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