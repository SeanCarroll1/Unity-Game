using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
using UnityEngine.UI;

namespace Completed

{

    public class BoardManager : MonoBehaviour
    {
        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        public class Count
        {
            public int minimum;             //Minimum value for our Count class.
            public int maximum;             //Maximum value for our Count class.


            //Assignment constructor.
            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }

         
            
        }
        // public Text texter;
      
       
        private int columns = 14;                                         //Number of columns in our game board.
        private int rows = 14;
        DifficultyLevel difficulty;

        private GameObject cellPrefab;
        public Cell[,] board=new Cell[14, 14];
       
    
        //Number of rows in our game board.
        public Count wallCount = new Count(5, 9);                       //Lower and upper limit for our random number of walls per level.
        public Count foodCount = new Count(1, 5);						//Lower and upper limit for our random number of food items per level.
        public Count healthCount = new Count(1, 5);
        public Count damageCount = new Count(1, 5);
       
        public GameObject exit;											//Prefab to spawn for exit.
        public GameObject damageTiles;
        public GameObject healthTiles;
        public GameObject floorTiles;                                   //Array of floor prefabs.
        public GameObject wallTiles;                                    //Array of wall prefabs.
        public GameObject foodTiles;                                    //Array of food prefabs.
        public GameObject enemyTiles;									//Array of enemy prefabs.
        public GameObject healthEnemyTiles;
        public GameObject SpeedEnemyTiles;
        public GameObject outerWallTiles;                               //Array of outer tile prefabs.

        private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
        private List<Vector3> gridPositions = new List<Vector3>();  //A list of possible locations to place tiles.

       
           
            // GUI.Box(new Rect(0, 0, 100, 50), "User logged in" + login.userName);
        

        void OnGUI()
        {
            if (login.userName != "")
            {
                GUI.Box(new Rect(0, 0, 160, 40), "User logged in: " + login.userName);
            }
            else
            {
                
                     GUI.Box(new Rect(0, 0, 200, 40), "User logged in: anonymous");
            }
        }

            //Sets up the outer walls and floor (background) of the game board.
            void BoardSetup()
        {
          
           // Debug.Log("user name=" + name);
            //Clear our list gridPositions.
            gridPositions.Clear();
           // OnGUI();


            //Loop along x axis, starting from - 1(to fill corner) with floor or outerwall edge tiles.
            for (int x = 0; x < columns ; x++)
            {
                //Loop along y axis, starting from -1 to place floor or outerwall tiles.
                for (int y = 0; y < rows ; y++)
                {
                    board[x, y] = new Cell() ;
                
                    //ensure distance of 3 from players starting point  
                    if (x < 4 && y < 4 )
                    {

                    }
                    else if(x > 0 && y > 0 && y < rows - 1 && x < columns - 1 && x!= rows - 2)
                    {
                      
                        //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                        gridPositions.Add(new Vector3(x, y, 0f));
                      

                    }
                   
                    //set prefab to outherwall to line the edges
                    if (x == 0 || x == columns-1 || y == 0 || y == rows-1)
                        cellPrefab = outerWallTiles;
                    else
                    {
                        //floor tiles otherwise
                        cellPrefab = floorTiles;
                    }

                    //Instantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject
                    GameObject cell = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                  
                
                   // fill in board with vectors as this will be used for pathfinding purposes
                    board[x, y].coordinates = new Vector2(x, y);
               
                }

        }
    }   
            
		
		
		
		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            int randomIndex = Random.Range(0, gridPositions.Count);

            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            Vector3 randomPosition = gridPositions[randomIndex];

            //Remove the entry at randomIndex from the list so that it can't be re-used.
            gridPositions.RemoveAt(randomIndex);

            //Return the randomly selected Vector3 position.
            return randomPosition;
        }
		
		
		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject tileArray, int minimum, int maximum)
		{
      
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray;

                //check if the tile is a wall, if so change tagname for pathfinding
				if(tileArray==wallTiles)
                {

                    board[(int)randomPosition.x, (int)randomPosition.y].tagName = "wall";
                }

                //if the tile is damage spawn health enemy on the same location for defending 
                else if (tileArray == damageTiles)
                {
                    Instantiate(healthEnemyTiles, randomPosition, Quaternion.identity); 
                }

                //Instantiate tile at random position
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

        //method to setup walls around map while guaranteeing there is a path to the exit
        void setupPaths(List<Cell> list)
        {
            //loop through pathfinding list
            for(int i=0; i<list.Count-1; i++)
            {
                //randomaly generate int to place wall
               int random= Random.Range(1, 3);
                Vector2 point = list[i].coordinates;
                Vector2 point2 = list[i].coordinates;
               //if  the path is heading up on the x axis add wall to y axis
                if (list[i+1].coordinates.x> list[i].coordinates.x)
                {
                    point.y = point.y + random;
                   
                }
                else
                {
                    point.x = point.x + random;
                   
                }
                //i % 5 != 0
                //ensure that walls do not block the exit from the player
                if (point.x<columns-1 && point.y<rows-1 && point2.y>0 && point2.x>0 && (int)point.x!=rows-2 && (int)point.y!=columns-2)
                {

                    Instantiate(wallTiles, point, Quaternion.identity);

                    //assign wall as a tagName at vector within board, this is for pathfinding purposes
                    board[(int)point.x, (int)point.y].tagName = "wall";
                    //removes vectors were the walls are placed
                    if(gridPositions.Contains(new Vector3((int)point.x, (int)point.y, 0f)))
                         gridPositions.RemoveAt( gridPositions.IndexOf(new Vector3((int)point.x, (int)point.y, 0f)));

                }
              

            }
        }

		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
			//Creates the outer walls and floor.
			BoardSetup ();
            difficulty = GameObject.Find("UI").GetComponent<DifficultyLevel>();
            //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);

            PathFinder path = new PathFinder();

            //find path from start to exit
            path.FindPath(board[1,1], board[rows-2, columns - 2], board, false);

            List<Cell> list = path.CellsFromPath();
            //setup wall cells around path
            setupPaths(list);

            Instantiate(exit, new Vector3(columns - 2, rows - 2, 0f), Quaternion.identity);

            //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

            //Instantiate a random number of health  tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(healthTiles, healthCount.minimum, healthCount.maximum);

           LayoutObjectAtRandom(damageTiles, damageCount.minimum, damageCount.maximum);

            //Determine number of enemies based on current level number, based on a logarithmic progression
            // int enemyCount = (int)Mathf.Log(level, 2f);
            // DifficultyLevel difficulty = GameObject.Find("UI").GetComponent<DifficultyLevel>();
            int enemyCount=2 ;
            //int difficulty;
           // Debug.Log("count " + PlayerPrefs.GetInt("Score") + " NUM " + enemyCount);

        
               
                if (difficulty.Easy)
                {
                    

                    enemyCount += 1;
                }
                else if (difficulty.medium)
                {
                   

                    enemyCount += 2;
                }
                // Debug.Log("count " + PlayerPrefs.GetInt("Score") + " NUM " + enemyCount);
            
           
           // Debug.Log("countlast " + PlayerPrefs.GetInt("Difficulty") + " NUM " + enemyCount);

            int healthEnemyCount = 3;
            //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
           
            //they are spawned on damage tiles
            //LayoutObjectAtRandom (healthEnemyTiles, healthEnemyCount, healthEnemyCount);
            LayoutObjectAtRandom(SpeedEnemyTiles,1 ,1);

            //Instantiate the exit tile in the upper right hand corner of our game board
          // Instantiate (exit, new Vector3 (columns - 2, rows - 2, 0f), Quaternion.identity);
		}
	}
}
