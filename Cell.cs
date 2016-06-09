using UnityEngine;
using System.Collections;
using Completed;

    public class Cell 
    {
        //allows us to read and write coordinates
        public Vector2 coordinates { get; set; }

        //add tag name for location
        public string tagName;

        // check if the path is walkable
        public virtual bool IsWalkable()
        {
            if(tagName=="wall"|| tagName == "SpeedEnemy"|| tagName == "HealthEnemy"|| tagName == "Enemy")
            {
                return false;
            }
            return true;
        }
      
    }

