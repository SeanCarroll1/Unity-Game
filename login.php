<?php
    // Database Things =========================================================
//These values can be found in the email.
    $host = "localhost"; //<--put your host here
    $user = "root"; //<-- put username here
    $password = ""; //<--put your password here
    $dbname = "scores"; //<-- put your database name here
    @mysql_connect($host, $user, $password) or die("Cant connect into database");
    mysql_select_db($dbname)or die("Cant connect into database");   
    // =============================================================================
    $Act = $_GET["Act"];// what is action, Login or Register?
    $nick = $_GET["User"];
    $pass = $_GET["Pass"];   
  // $Email = $_GET["Email"];    
    if($Act == "Login"){
    if(!$nick || !$pass) {
        echo "Login or password cant be empty.";
        } else {
        $SQL = "SELECT * FROM scores WHERE name = '" . $nick . "'";
        $result_id = @mysql_query($SQL) or die("DATABASE ERROR!");
        $total = mysql_num_rows($result_id);
        if($total) {
            $datas = @mysql_fetch_array($result_id);
            if(!strcmp($pass, $datas["password"])) {
                echo "Correct ";
				echo $nick;
            } else {
                echo "Wrong";
            }
        } else {
            echo "No User";
        }
    } 
    }
    
   if($Act == "Register"){
       
        $checkuser = mysql_query("SELECT name FROM scores WHERE name='$nick'"); 
        $username_exist = mysql_num_rows($checkuser);
        if($username_exist > 0)
        {
              echo "TAKEN";// Username is taken
              
              unset($nick);
              exit();
        }else{
            $query = "INSERT INTO scores (name, password,highscore) VALUES('$nick', '$pass',0)";
            mysql_query($query) or die("ERROR");
            mysql_close();
            echo "Registered ";
			echo $nick;
			   exit();
        }
    }
    // Close mySQL Connection
    mysql_close();
    ?>