using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Completed;

namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Enemy : MovingObject
	{
		//public int playerDamage; 							//The amount of food points to subtract from the player when attacking.
		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.
        private Text enemyHealth;

        BoardManager bScript;
        Player player;
        private Canvas canvas;
        private int turnCount=0;
        int hp = 5;
        int highHp = 10;
        int lowHp = 3;
        private int normalAttack = 2,HealthAttack=1;

        private Slider healthBar;
        private Text enemiesKilledText;

        [HideInInspector]
        private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.
		private Transform target,targetDamage;							//Transform to attempt to move toward each turn.
		private bool skipMove=false;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.
       
		
		//Start overrides the virtual Start function of the base class.
		protected override void Start ()
		{
			//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
			//This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList (this);
            
            enemyHealth = GameObject.Find("EnemyText").GetComponent<Text>();
            player = GameObject.Find("Player").GetComponent<Player>();
            bScript = GameObject.Find("GameManager(Clone)").GetComponent<BoardManager>();
            //healthBar = GameObject.Find("EnemyHealth").GetComponent<Slider>();
            //canvas = GameObject.Find("HealthPosition").GetComponent<Canvas>();
            enemiesKilledText = GameObject.Find("EnemiesKilled").GetComponent<Text>();
            //Get and store a reference to the attached Animator component.
            animator = GetComponent<Animator> ();
        
            //set health bar according to enemy type
           /* if (tag == "Enemy")
            {
                healthBar.value = hp;
                healthBar.maxValue = hp;
            }
            else if (tag == "HealthEnemy")
            {
                healthBar.value = highHp;
                healthBar.maxValue = highHp;
            }
            else
            {
                healthBar.value = lowHp;
                healthBar.maxValue = lowHp;     
            }*/
           // Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            //canvas.worldCamera = screenPos;
           // canvas.position
            // healthBar.transform
            //Find the Player GameObject using it's tag and store a reference to its transform component.
            target = GameObject.FindGameObjectWithTag ("Player").transform;
            enemiesKilledText.text = "Enemies Defeated: " + player.enemiesDefeated;
            //targetDamage = GameObject.FindGameObjectWithTag("SpeedEnemy").transform;
            //Call the start function of our base class MovingObject.
            base.Start ();
		}

        private void Update()
        {

           // healthBar.maxValue = hp;
            // playerHealth.maxValue = 300;
            // playerHealth.value = health;
        }
		
		//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
            
            //Check if skipMove is true, if so set it to false and skip this turn.
            if (player.extraTurn > 0)
            {
                Debug.Log("first place" + turnCount + " " + player.extraTurn);

                if (turnCount>0)
                {
                    Debug.Log("right place");
                    turnCount--;
                    return;
                }else
                {
                    turnCount = player.extraTurn;
                    Debug.Log("reset place");

                }

            }
            Debug.Log("move place");

            //Call the AttemptMove function from MovingObject.
            base.AttemptMove <T> (xDir, yDir);
			
			//Now that Enemy has moved, set skipMove to true to skip next move.
			//skipMove = true;
		}


        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void MoveEnemy (String tag)
		{
			//Declare variables for X and Y axis move directions, these range from -1 to 1.
			//These values allow us to choose between the cardinal directions: up, down, left and right.
			int xDir = 0;
			int yDir = 0;
            
            //create two cells for origin and goal
            Cell origin = new Cell();
            Cell goal =new Cell();
            
            //set coordinates to position of gameobject currently in use
            origin.coordinates = new Vector2(transform.position.x, transform.position.y);

            setEnemyLocations(true);

             //create pathfinder object
             PathFinder pathFinder = new PathFinder();

            //check if the enemy being used is the healthenemy and there are still some damage objects left
            if (tag == "HealthEnemy" && GameObject.FindGameObjectWithTag("Damage") != null)
            {

                //create array of gameobjects
                GameObject[] gos;

                //find all damage gameObjects
                gos = GameObject.FindGameObjectsWithTag("Damage");


                //check if closest method null
                if (closest() != null)
                {
                    //check if the distance betweenthe player and the nearest damage tile is less then 5
                    if ((Mathf.Abs(target.position.x - closest().transform.position.x) + Mathf.Abs(target.position.y - closest().transform.position.y)) < 5)
                    {
                        //set goal coordinates to the player location
                        goal.coordinates = new Vector2(target.position.x, target.position.y);
                    }
                    else
                    {
                        //set goal to the closest damage tile 
                        goal.coordinates = new Vector2(closest().transform.position.x, closest().transform.position.y);
                    }
                }
            }
            //if there are no damage tiles or none that are free
            else
            {
                //set goal as the players location
                goal.coordinates = new Vector2(target.position.x, target.position.y);
            }
               

                //set pathfinder object using the orgin and goal cell, the bscript.board from boardManager 
                pathFinder.FindPath(origin, goal, bScript.board, false);
                
                //store the pathfinding list
                List<Cell> list = pathFinder.CellsFromPath();

                //if the list is bigger then zero
                if (list.Count>0)
                {
                    //add the first cell in the list to the vector            
                    Vector2 end = list[0].coordinates;
                    //calulate the x and y to check which direction to move
                    xDir = (int)end.x - (int)transform.position.x;
                    yDir = (int)end.y - (int)transform.position.y;
                }

            //check if it is a speedenemy and if the list has two cells
            if (tag == "SpeedEnemy" && list.Count > 1)
            {
                Vector2 end = list[1].coordinates;
                //find the second cell
                //move it to the new location alowing the speed enemy to move two cells each turn
                AttemptMove<Player>((int)end.x - (int)transform.position.x, (int)end.y - (int)transform.position.y);
            }

            AttemptMove<Player>(xDir, yDir);

            
          
            setEnemyLocations(false);

            

        }

        void setEnemyLocations(bool start)
        {
                //find the current locations of speedEnemys in the board
                GameObject[] fastEnemy;
                if (GameObject.FindGameObjectWithTag("SpeedEnemy") != null)
                {
                    fastEnemy = GameObject.FindGameObjectsWithTag("SpeedEnemy");

                    foreach (GameObject go in fastEnemy)
                    {
                        Vector3 pos = go.transform.position;
                        //add tag name to location for pathfinding purposes
                        if(start)
                        {
                            bScript.board[(int)pos.x, (int)pos.y].tagName = "wall";
                        }
                        else
                        {
                            bScript.board[(int)pos.x, (int)pos.y].tagName = null;
                        }
                    
                    }
                }

                //find the current locations of healthenemy in the board
                GameObject[] healthEnemy;
                if (GameObject.FindGameObjectWithTag("HealthEnemy") != null)
                {
                    healthEnemy = GameObject.FindGameObjectsWithTag("HealthEnemy");

                    foreach (GameObject go in healthEnemy)
                    {
                        Vector3 pos = go.transform.position;
                        //add tag name to location for pathfinding purposes
                        if (start)
                        {
                            bScript.board[(int)pos.x, (int)pos.y].tagName = "wall";
                        }
                        else
                        {
                            bScript.board[(int)pos.x, (int)pos.y].tagName = null;
                        }
                    }
                 }

                //find the current locations of normal enemy in the board
                GameObject[] Enemy;
                if (GameObject.FindGameObjectWithTag("Enemy") != null)
                {
                    Enemy = GameObject.FindGameObjectsWithTag("Enemy");

                    foreach (GameObject go in Enemy)
                    {
                        Vector3 pos = go.transform.position;
                        //add tag name to location for pathfinding purposes
                        if (start)
                        {
                            bScript.board[(int)pos.x, (int)pos.y].tagName = "wall";
                        }
                        else
                        {
                            bScript.board[(int)pos.x, (int)pos.y].tagName = null;
                        }
                    }
                }
        }

        //method to find the closest damage tile
        public GameObject closest()
        {
            GameObject[] gos;

            gos = GameObject.FindGameObjectsWithTag("Damage");

            float distance = Mathf.Infinity;
            // Vector3 position = transform.position;
            GameObject closestObject = null;
            foreach (GameObject go in gos)
            {
                //find the distance from the damage tile to the enemy position
                Vector3 diff = go.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;

                //if the enemy is already on the damage tile return
                if (go.transform.position == transform.position)
                {
                    //closestObject = go;
                   // distance = curDistance;
                    //return the tile
                    return go;
                }
                //if distance is shorter
                else if (curDistance < distance)
                {


                    distance = curDistance;
                    if (bScript.board[(int)go.transform.position.x, (int)go.transform.position.y].tagName == null)
                    {
                        //set closest tile 
                        closestObject = go;
                    }

                }


            }
            return closestObject;
        }


        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
        protected override void OnCantMove <T> (T component,String tag)
		{
			//Declare hitPlayer and set it to equal the encountered component.
			Player hitPlayer = component as Player;

            //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
            if (tag == "Enemy")
            {
                normalAttack= normalAttack+ GameManager.level;
                hitPlayer.LoseFood(normalAttack);
            }
			else if(tag=="HealthEnemy")
            {
                hitPlayer.LoseFood(HealthAttack);
            }
            else
            {
                hitPlayer.LoseFood(normalAttack);
            }
           
			
           
		}

        //DamageEnemy is called when the player attacks a enemy.
        public void DamageEnemy(int loss)
        {
           
            if (tag=="Enemy")
            {
               // healthBar.value -= loss;
                enemyHealth.text = "Enemy health " + hp;
                hp -= loss;
                //player.player++;
                //If hit points are less than or equal to zero:
                if (hp <= 0)
                {

                    player.score = player.score + 5;
                    player.enemiesDefeated += 1;
                    enemiesKilledText.text = "Enemies Defeated: " + player.enemiesDefeated;
                    //Disable the gameObject.
                    gameObject.SetActive(false);
                   // enemiesDefeated++;
                }
                   
            }
            else if(tag=="HealthEnemy")
            {
                enemyHealth.text = "Enemy health " + highHp;
                //Subtract loss from hit point total.
                highHp -= loss;
               // healthBar.value -= loss;
              //  player.player++;
                //If hit points are less than or equal to zero:
                if (highHp <= 0)
                {
                    //Disable the gameObject.
                    player.enemiesDefeated += 1;
                    enemiesKilledText.text = "Enemies Defeated: " + player.enemiesDefeated;
                    player.score = player.score + 5;
                    gameObject.SetActive(false);
                }
                    
            }
            else
            {
                enemyHealth.text = "Enemy health " + lowHp;
                //Subtract loss from hit point total.
                lowHp -= loss;
               // healthBar.value -= loss;
                // player.player++;
                //If hit points are less than or equal to zero:
                if (lowHp <= 0)
                {
                    player.score = player.score + 5;
                    player.enemiesDefeated += 1;
                    enemiesKilledText.text = "Enemies Defeated: " + player.enemiesDefeated;
                    //Disable the gameObject.
                    gameObject.SetActive(false);
                }
                    
            }
           
        }

        protected override void enemyHit<T>(T component)
        {
            throw new NotImplementedException();
        }
    }
}
