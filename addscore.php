<?php
    // Database Things =========================================================
//These values can be found in the email.
    $host = "localhost"; //<--put your host here
    $user = "root"; //<-- put username here
    $password = ""; //<--put your password here
    $dbname = "scores"; //<-- put your database name here
    @mysql_connect($host, $user, $password) or die("Cant connect into database");
    mysql_select_db($dbname)or die("Cant connect into database");   
	$nick = $_GET["User"];
	$score = $_GET["highscore"]; 
	 $checkuser = mysql_query("SELECT name, highscore FROM scores WHERE name='$nick'"); 
        $username_exist = mysql_num_rows($checkuser);
        if($username_exist > 0)
        {
               while ($row = mysql_fetch_array($result)) {
					if ($row['highscore']<$score)
					{
						 $sql = mysql_query("UPDATE `scores` SET `score` = '$score' WHERE `name` = '$name'");
					}
        }
	
	if ($sql){
      //The query returned true - now do whatever you like here.
      echo 'Your score was saved. Congrats!';
 } else {    
      //The query returned false - you might want to put some sort of error reporting here. Even logging the error to a text file is fine.
      echo 'There was a problem saving your score. Please try again later.'.mysql_error();
	}
	  // Close mySQL Connection
    mysql_close();
    ?>