-- CREATE DATABASE animal;

-- create user table
CREATE TABLE user 
  ( 
     uid              MEDIUMINT PRIMARY KEY auto_increment, 
     username         VARCHAR(20) NOT NULL, 
     pass             VARCHAR(200) NOT NULL, 
     email            VARCHAR(100), 
     created_time     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
     last_login_time  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
     last_logout_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
     logged_in        BOOL, 
     gid              INT NOT NULL DEFAULT 0,
     want_play        BOOL
  ); 

ALTER TABLE user 
  ADD INDEX (logged_in), 
  ADD UNIQUE index (username), 
  ADD INDEX (gid); 

CREATE TABLE game 
  ( 
     gid         INT PRIMARY KEY auto_increment, 
     p1          MEDIUMINT, 
     p2          MEDIUMINT, 
     board       VARCHAR(3000), 
     turn       INT NOT NULL DEFAULT 0, 
     p1_color    TINYINT(1), 
     game_status VARCHAR(1), 
     completed   BOOL, 
     winner      MEDIUMINT, 
     start_time  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
     end_time    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
     FOREIGN KEY (p1) REFERENCES user(uid), 
     FOREIGN KEY (p2) REFERENCES user(uid), 
     FOREIGN KEY (winner) REFERENCES user(uid) 
  ); 

ALTER TABLE game 
  ADD INDEX (p1), 
  ADD INDEX (p2), 
  ADD INDEX (winner), 
  ADD INDEX (game_status); --- w/f (winning/no winner)


  --- commands

  -- SELECT  a.*, 
  --       b.username as p1Name,
  --       c.username as p2Name      
  -- FROM    game a
  --       INNER JOIN user b
  --           ON  a.p1 = b.uid 
  --       INNER JOIN user c
  --           ON  a.p2 = c.uid



  -- source /home/wei/Desktop/animal/backend/SqlQuery.sql
