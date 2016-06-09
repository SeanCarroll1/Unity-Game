<?php
    // create the connection to our database with following values: location of our databse
    // (with xampp it's "localhost"), next is the login ("name" and "password").
    // if the connection can not be established we get an error message, that we've entered after "or die"
    $host = "localhost"; //<--put your host here
    $user = "root"; //<-- put username here
    $password = ""; //<--put your password here
    $dbname = "scores"; //<-- put your database name here
    @mysql_connect($host, $user, $password) or die("Cant connect into database");
    mysql_select_db($dbname)or die("Cant connect into database"); 
    
    // now we simply get the scores and sort them by their value. we also add a limit of 5, so we only
    // select the 5 highest values. The * means we search through every value.
    $query = "SELECT * FROM scores ORDER by highscore DESC LIMIT 5";
    
    // now we store our selected values into a result variable
    $result = mysql_query($query);
    
    // this will select the whole row we found the score at
    $num_results = mysql_num_rows($result);  
 
    // at the end we will get 5 rows with only the name and score values in each row
    for($i = 0; $i < $num_results; $i++)
    {
         $row = mysql_fetch_array($result);
         // the echo command is used as the returned value for our Unity Script
         echo $row['name'] . "\t" . $row['highscore'] . "\n";
    }

    // we're done now, so we can close the connection
    mysql_close();
?>