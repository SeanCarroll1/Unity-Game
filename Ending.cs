using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using System;
using UnityEngine.SceneManagement;

namespace Completed
{
 
    public class Ending : MonoBehaviour
    {
        public int score;
        public string highscore_url = "http://localhost/phpmyadmin/getTopScores.php";
        public string addScore = "http://localhost/phpmyadmin/addscore.php";
        private Text levelText;
        private Text foodText;
        private Text scoreText;
        private Text highScoreText;
        private Text enemiesKilledText;
        private WWW download;
        // Use this for initialization
        void Start()
        {
            
            scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
            highScoreText = GameObject.Find("HighScore").GetComponent<Text>();
            enemiesKilledText = GameObject.Find("EnemiesKilled").GetComponent<Text>();

            levelText = GameObject.Find("LevelText").GetComponent<Text>();

            //Set the text of levelText to the string "Day" and append the current level number.
            levelText.text = "Level " + GameManager.level;

            // foodText.text = " Health: " + health + " Damage: " + damage;
            enemiesKilledText.text = "Enemies Defeated : " +GameManager.enemiesDefeated;
            scoreText.text = "Score: " + GameManager.score;
            highScoreText.text = "High Score : " + PlayerPrefs.GetInt("highscore", 0);
          
            StartCoroutine(scores());
            GameManager.score = 0;

        }

        IEnumerator scores()
        {
            // Create a form object for sending high score data to the server
            WWWForm form = new WWWForm();
            // Assuming the perl script manages high scores for different games
           // form.AddField("game", "MyGameName");
            // The name of the player submitting the scores
            form.AddField("name", login.userName);
            // The score
            form.AddField("highscore", score);
            string add = addScore + "?User=" + login.userName + "&highscore=" + GameManager.score;

            WWW insert =new WWW(add);
            yield return insert;

            // Create a download object
            download = new WWW(highscore_url, form);

            // Wait until the download is done
            yield return download;

            if (!string.IsNullOrEmpty(download.error))
            {
                print("Error downloading: " + download.error);
            }
            else {
                // show the highscores
                Debug.Log(download.text);
            }
        }

        void OnGUI()
        {
            if(download!=null)
            {
                GUI.Box(new Rect(0, 0, 160, 140),"High Scores "+ "\n \n" + download.text);
            }
           
          
        }

       
    }
}