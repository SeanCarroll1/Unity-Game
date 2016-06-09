using UnityEngine;
using System.Collections;

namespace Completed

{
    public class DifficultyLevel : MonoBehaviour
    {

        [HideInInspector]
        public bool Easy = true;

        [HideInInspector]
        public bool medium = false;
    
        //when Easy is clicked 
        public void SetEasy()
        {

           
                Easy = true;
                medium = false;
             
               
        }

        //when medium is clicked
        public void SetMedium()
        {
           
                medium = true;
                Easy = false;

              


        }
    }
}