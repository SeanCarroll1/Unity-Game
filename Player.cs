using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using System;
using UnityEngine.SceneManagement;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{

		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int pointsPerFood = 2;				//Number of points to add to player food points when picking up a food object.
		public int pointsPerSoda = 5;              //Number of points to add to player food points when picking up a soda object.
        public int damagePower = 2;
        [HideInInspector]
        public int extraTurn=0;
        [HideInInspector]
        public float timeLeft = 120.0f;
        DifficultyLevel difficulty;
        private bool done = false;
   
       // private Slider playerHealth;
        int Num = 0;
        private Text foodText;						//UI Text to display current player food total.
        private Text timeLimit;
        private Text scoreText;
        private Text highScoreText;

        public int enemiesDefeated;
        private Animator animator;					//Used to store a reference to the Player's animator component.
		private int health;							//Used to store player food points total during level.
        private int damage;
		private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
        [HideInInspector]
        public int score;
     

        //Start overrides the Start function of MovingObject
        protected override void Start ()
		{
            //Get a component reference to the Player's animator component
           // diff = GameObject.Find("DifficultyLevel").GetComponent<Player>();
           
            animator = GetComponent<Animator>();
            //playerHealth = GameObject.Find("PlayerHealth").GetComponent<Slider>();
            //foodText=GetComponent<Text>();
            foodText = GameObject.Find("FoodText").GetComponent<Text>();
            scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
            highScoreText = GameObject.Find("HighScore").GetComponent<Text>();
            timeLimit = GameObject.Find("TimeLimit").GetComponent<Text>();
            difficulty = GameObject.Find("UI").GetComponent<DifficultyLevel>();
            //Get the current health,damage total and score stored in GameManager.instance between levels.
            health = GameManager.instance.health;
            damage = GameManager.playerDamage;
            score = GameManager.score;
            enemiesDefeated= GameManager.enemiesDefeated;

           // playerHealth.maxValue = 300;
           // playerHealth.value = health;

            score =score+ 100;
            score += health;

            if (difficulty.medium)
            {
                timeLeft = 90.0f;
            }else
                {
                timeLeft = 120.0f;
            }
         
                //Set the foodText to reflect the current player food total.
                foodText.text = " Health: " + health + " Damage: " + damage;
            scoreText.text = "Score: " + score;
            highScoreText.text ="High Score : "+ PlayerPrefs.GetInt("highscore", 0);
            //Call the Start function of the MovingObject base class.
            base.Start ();
		}

    
        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {
            //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
            GameManager.instance.health = health;
            GameManager.playerDamage = damage;
            GameManager.score = score;
            GameManager.enemiesDefeated = enemiesDefeated;
        }


        private void Update ()
		{
            timeLeft -= Time.deltaTime;
            timeLimit.text = "Time remaning "+(int)timeLeft;

            if (timeLeft < 0)
            {
                CheckIfGameOver();
            }
            //If it's not the player's turn, exit the function.
            if (!GameManager.instance.playersTurn )
            {

              //  speedPickup = totalspeed;
                return;
            }
           // extraTurn = speedPickup;
            foodText.text = " Health: " + health + " Damage: " + damage;
            scoreText.text = "Score: " + score;

            int horizontal = 0;  	//Used to store the horizontal move direction.
			int vertical = 0;       //Used to store the vertical move direction.
            //Check if we are running either in the Unity editor or in a standalone build.
            #if UNITY_STANDALONE || UNITY_WEBPLAYER

            //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
            horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}

		
			#endif //End of mobile platform dependendent compilation section started above with #elif
			//Check if we have a non-zero value for horizontal or vertical
			if(horizontal != 0 || vertical != 0)
			{
				//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
				//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
				AttemptMove<Enemy> (horizontal, vertical);
			}
		}

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        protected override void AttemptMove<T>(int xDir, int yDir)
        {
         
              
                base.AttemptMove<T>(xDir, yDir);

                //Hit allows us to reference the result of the Linecast done in Move.
                RaycastHit2D hit;

                //If Move returns true, meaning Player was able to move into an empty space.
                if (Move(xDir, yDir, out hit))
                {
                    //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
                    //SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
                }

                //Since the player has moved and lost health , check if the game has ended.
                CheckIfGameOver();
      
        
                GameManager.instance.playersTurn = false;
          
              
        
            
        }

  
        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
        protected override void enemyHit<T> (T component)
		{
            Enemy hit = component as Enemy;
            //Set hitWall to equal the component passed in as a parameter.
            //Wall hitWall = component as Wall;
            String tag = component.tag;
            scoreText.text = "Score: " + score;
            hit.DamageEnemy(damage);
           
            //foodText.text = "tester";
           
            //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
            //animator.SetTrigger ("playerChop");
        }

       

        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if(other.tag == "Exit")
			{
				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				Invoke ("Restart", restartLevelDelay);
				
				//Disable the player object since level is over.
				//enabled = false;
			}
			
			//Check if the tag of the trigger collided with is Food.
			else if(other.tag == "Food")
			{
                //Add pointsPerFood to the players current food total.
                health += pointsPerFood;
                score += pointsPerFood;
                //Update foodText to represent current total and notify player that they gained points
                foodText.text = "+ " + pointsPerFood + " Added Health "+ " Health: " + health + " Damage: " + damage;

                //Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
                //SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
				//Disable the food object the player collided with.
				other.gameObject.SetActive (false);
			}
        
            //Check if the tag of the trigger collided with is Soda.
            else if(other.tag == "Soda")
			{
                //Add pointsPerSoda to players food points total
                //health += pointsPerSoda;
                //score += pointsPerSoda;
                if(extraTurn <= 2)
                {
                    extraTurn++;
                }
              
                //Update foodText to represent current total and notify player that they gained points
               // foodText.text = "+ " + pointsPerSoda + " Added Health " + " Health: " + health + " Damage: " + damage;

                //Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
               // SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				
				//Disable the soda object the player collided with.
				other.gameObject.SetActive (false);
			}
            else if (other.tag == "Damage")
            {
                //Add damage to total Damage.
                damage += damagePower;

                //Update text to represent total health and damage.
                foodText.text = "+ " + damagePower+" Added Damage " + " Health: " + health + " Damage: " + damage;

                //Disalbe the damagePower object the player collided with.
                other.gameObject.SetActive(false);
            }

        }
		
		
		//Restart reloads the scene when called.
		private void Restart ()
		{
            SceneManager.LoadScene(1);
            //Load the last scene loaded, in this case Main, the only scene in the game.
           // Application.LoadLevel (Application.loadedLevel);
		}
		
		
		//LoseFood is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseFood (int loss)
		{
            //Set the trigger for the player animator to transition to the playerHit animation.
            //animator.SetTrigger ("playerHit");

            //Subtract lost food points from the players total.
            health -= loss;
            score -= loss;
			//Update the food display with the new total.
			foodText.text = "-"+ loss + " Health: " + health + " Damage: " + damage;
            
            //Check to see if game has ended.
            CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
			if (health <= 0 || timeLeft<=0) 
			{
                //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
                //SoundManager.instance.PlaySingle (gameOverSound);

                //Stop the background music.
                //SoundManager.instance.musicSource.Stop();
               
                
                
                    int oldHighscore = PlayerPrefs.GetInt("highscore", 0);
                
               
                if (score > oldHighscore)
                    PlayerPrefs.SetInt("highscore", score);
                Debug.Log(oldHighscore);
                //Call the GameOver function of GameManager.
                GameManager.instance.GameOver ();
                SceneManager.LoadScene(2);
            }
		}

        protected override void OnCantMove<T>(T component,String tag)
        {
            throw new NotImplementedException();
        }
    }
}

