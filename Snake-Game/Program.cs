using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Media;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Snake
{
    struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    class Program
    {
        public static bool SoundMute = false;

        //Method Draws Obstacles
        static void DrawObstacles(List<Position> obstacles)
        {
            foreach (Position i in obstacles)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(i.col, i.row);
                Console.Write("=");
            }
        }

        //Method Draws Food
        static void DrawFood(List<Position> food)
        {
            int foodpoint = 1;
            foreach (Position i in food)
            {
                Console.SetCursorPosition(i.col, i.row);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(foodpoint);
                foodpoint++;
            }
        }

        //Method Draws Snake
        static void DrawSnake(Queue<Position> snakeElements)
        {
            foreach (Position i in snakeElements)
            {
                Console.SetCursorPosition(i.col, i.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");
            }
        }

        //Method Checks Direction
        static int InputCheck(byte up, byte down, byte left, byte right, int direction, bool pause, List<string> PauseMenu)
        {
            while (Console.KeyAvailable)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(68,1);
                ConsoleKeyInfo userInput = Console.ReadKey();
                Console.SetCursorPosition(68, 1);
                Console.WriteLine(" ");
                if (userInput.Key == ConsoleKey.LeftArrow)
                {
                    if (direction != right) direction = left;
                }
                else if (userInput.Key == ConsoleKey.RightArrow)
                {
                    if (direction != left) direction = right;
                }
                else if (userInput.Key == ConsoleKey.UpArrow)
                {
                    if (direction != down) direction = up;
                }
                else if (userInput.Key == ConsoleKey.DownArrow)
                {
                    if (direction != up) direction = down;
                }
                else if (userInput.Key == ConsoleKey.Enter && pause == false)
                {
                    Console.Clear();
                    MainMenu(PauseMenu, "", 0, 0,false);
                }
            } return direction;
        }

        //Method Displays Name Request Screen
        static string NameScreen()
        {
            string PlayerName, Message;
            Console.Clear();
            for (;;)
            {
                Console.SetCursorPosition(43, 16);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please Enter Your Name and Press Enter");
                Console.SetCursorPosition(57, 17);
                PlayerName = Console.ReadLine();

                if (PlayerName == "") Message = "   You Must Enter Your Name To Play \n\t\t\t\t\t   (Press Enter to Input Your Name Again)";
                else if (!Regex.IsMatch(PlayerName, @"^[a-zA-Z]+$")) Message = "Your Name Can Only Contain Alphabets \n\t\t\t\t\t  (Press Enter to Input Your Name Again)";
                else if (PlayerName.Length > 12) Message = "Your Name Can Not Be More Than 12 Characters Long \n\t\t\t\t\t      (Press Enter to Input Your Name Again)";
                else if (PlayerName.Length < 3) Message = "Your Name Must Be At Least 3 Characters Long \n\t\t\t\t\t      (Press Enter to Input Your Name Again)";
                else break;

                Console.Clear();
                Console.SetCursorPosition(43, 16);
                Console.WriteLine(Message);
                Console.ReadKey();
                Console.Clear();
            }

            Console.Clear();
            return PlayerName;
        }

        //Method Calculates Total Score for the player then Outputs Name & Total Score to Text File
        static void WriteFile(string PlayerName, int userPoints)
        {
            bool NameExists = false;
            int x = 0, TotalScore;
            string FileName = @"Records\records.txt";
            string[] Line = File.ReadAllLines(FileName);

            foreach (string i in Line)
            {
                string name = (Regex.Replace(i, "[^a-zA-Z]", ""));              //Finds Name in the line and adds to NameList
                if (name == PlayerName)
                {
                    TotalScore = Int32.Parse(Regex.Replace(i, "[^0-9]", ""));   //Finds Score in the line
                    TotalScore = TotalScore + userPoints;                       //Totals Score
                    Line[x] = PlayerName + " " + TotalScore;                    //Saves Name and Total Score to Line Array
                    NameExists = true;
                }
                x++;
            }

            File.WriteAllText(FileName, String.Empty);                          //Deletes all data in Text File

            foreach (string i in Line)                                          //Adds Each Line to the file
            {
                File.AppendAllText(FileName, i + Environment.NewLine);
            }

            if (NameExists == false)                                            //Adds New Player's Score to file
            {
                File.AppendAllText(FileName, PlayerName + " " + userPoints + Environment.NewLine);
            }
        }

        //Displays Score Board
        static void DisplayScoreBoard()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(40, 1);
            Console.Write("Player Name");
            Console.SetCursorPosition(70, 1);
            Console.Write("Total Score");
            Console.SetCursorPosition(0, 2);
            Console.WriteLine("=============================================================================================================================");

            string FileName = @"Records\records.txt";
            string[] Line = File.ReadAllLines(FileName);
            List<string> Name = new List<string>();
            List<string> Score = new List<string>();

            foreach (string i in Line)
            {
                Name.Add(Regex.Replace(i, "[^a-zA-Z]", ""));
                Score.Add(Regex.Replace(i, "[^0-9]", ""));
            }

            for (int i = 0; i < Name.Count; i++)
            {
                Console.SetCursorPosition(40, i + 3);
                Console.Write(Name[i]);
                Console.SetCursorPosition(70, i + 3);
                Console.Write(Score[i]);
            }

            Console.SetCursorPosition(36, 31);
            Console.Write("\t\t[Press 'ENTER' To Go Back]");
            
            for (;;) 
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(0, 32);
                ConsoleKeyInfo presskey = Console.ReadKey();
                Console.SetCursorPosition(0, 32);
                Console.WriteLine(" ");
                if (presskey.Key == ConsoleKey.Enter) break;
            }
        }

        //Displays Main Menu
        static void MainMenu(List<string> menuOpts, string statement, int? pointsGet, int? pointsAim, bool noPoints)
        { 
            int index = 0;
            string result = "";
            
            while (true)
            {  
                if (SoundMute == true) menuOpts[4] = "|   Play Music   |";
                else menuOpts[4] = "|   Mute Music   |";
                if (pointsGet >= pointsAim && noPoints == true)                 //Points that reached the winning score wins the game
                {
                    result = "YOU WIN!";                       //to show the result 'GAME OVER!' after player gets 0 points
                }
                else if (pointsGet <= pointsAim)             //if points lower or equal than expected points to win loses the game and show the result 'GAME OVER!'
                {
                    if ((pointsGet == 0 || pointsAim == 0) && noPoints == false)  //Points will be null if its 0 points for both startMenu and Pausemenu
                    {
                        pointsAim = pointsGet = null; 
                        result = "";
                    }else if ((pointsGet == 0 || pointsAim == 0) && noPoints == true){            //to show the result 'GAME OVER!' after player gets 0 points
                        result = "GAME OVER!";                       
                    }else {result = "GAME OVER!";}
                }
                
                Console.Clear();
                Console.SetCursorPosition(59,10); 
                Console.ForegroundColor = ConsoleColor.Red;
                //statement results for only back menu
                Console.WriteLine(result +"\n\t\t\t\t\t\t     "+statement);
                //prints out the game title
                Console.SetCursorPosition(41, 2);
                Console.WriteLine("   ███████╗███╗   ██╗ █████╗ ██╗  ██╗███████╗");
                Console.SetCursorPosition(41, 3);
                Console.WriteLine("   ██╔════╝████╗  ██║██╔══██╗██║ ██╔╝██╔════╝");
                Console.SetCursorPosition(41, 4);
                Console.WriteLine("   ███████╗██╔██╗ ██║███████║█████╔╝ █████╗");
                Console.SetCursorPosition(41, 5);
                Console.WriteLine("   ╚════██║██║╚██╗██║██╔══██║██╔═██╗ ██╔══╝");
                Console.SetCursorPosition(41, 6);
                Console.WriteLine("   ███████║██║ ╚████║██║  ██║██║  ██╗███████╗");
                Console.SetCursorPosition(41, 7);
                Console.WriteLine("   ╚══════╝╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝");
                //List print out options for user to select by pressing up and down keys
                for (int i = 0; i < menuOpts.Count; i++)
                {   
                    Console.SetCursorPosition(55,13 + i); 
                    if (i == index)
                    {
                        //selecton background colour
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        //foreground
                        Console.ForegroundColor = ConsoleColor.Black; 
                    }
                    else
                    {
                        Console.ResetColor();
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(menuOpts[i]);//prints out the list
                }

                //obtains the next character or any key pressed by the user.  
                ConsoleKeyInfo presskey = Console.ReadKey();

                //moves selection down by index
                if (presskey.Key == ConsoleKey.DownArrow)
                {
                    if (index == menuOpts.Count - 1)
                    {
                        index = 0;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        index += 2;
                    }

                    //moves selection up by index
                }
                else if (presskey.Key == ConsoleKey.UpArrow)
                {
                    if (index <= 0)
                    {
                        index = menuOpts.Count - 1;
                    }
                    else
                    {
                        index -= 2;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
                else if (presskey.Key == ConsoleKey.Enter)
                {
                    //Start to proceed to the game
                    if (index == 0)
                    {
                        Console.Clear();
                        return;
                    }
                    else if (index == 2)
                    {
                        DisplayScoreBoard();
                    }
                    else if (index == 4)  //select 'Mute' to mute the program
                    {
                        if (SoundMute == true) 
                        { 
                            SoundMute = false;
                            menuOpts[4] = "|   Mute Music   |";
                        }
                        else
                        { 
                            SoundMute = true;
                            menuOpts[4] = "|   Play Music   |";
                        }
                        PlayMusic("Background");
                    }
                    else if (index == 6)  //select 'Exit' to close the program
                    {
                        Environment.Exit(0);
                    }
                }
                else {Console.BackgroundColor = ConsoleColor.Black;
                    //to prevent background from turning blue when player accidentally presses other keys (left and right keys)                 
                }
            }Console.Clear();
        }

        //Music plays or mute as the player so desires to choose
        static void PlayMusic(string SoundType)
        {
            //different variable type of sounds 
            var BiteSound = new SoundPlayer(); BiteSound.SoundLocation = @"Sounds\Bite.wav";
            var DamageSound = new SoundPlayer(); DamageSound.SoundLocation = @"Sounds\Damage.wav";
            var GameOverSound = new SoundPlayer(); GameOverSound.SoundLocation = @"Sounds\GameOver.wav";
            var BackgroundSound = new SoundPlayer(); BackgroundSound.SoundLocation = @"Sounds\Background.wav";

            if (SoundMute == true)                      //if player selects 'Mute Music' the music stops
            { 
                BackgroundSound.Stop(); 
            }
            else
            {
                if (SoundType == "Background")          //background music will loop throughout the whole game
                {
                    BackgroundSound.PlayLooping();
                }
                else if (SoundType == "GameOver")       //game over music
                {
                    GameOverSound.Play();
                }
                else if (SoundType == "Damage")         //sound effects for hitting an obstacle or biting its own body
                {
                    DamageSound.Play();
                }
                else if (SoundType == "Bite")           //sound effects of a bite sound when the snake eats food
                {
                    BiteSound.Play();
                }
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "Snake";
            while (true)
            {
                double sleepTime = 100;              //SleepTime indicates speed movement of the snake, the higher the number, the slower the speed of the snake
                byte right = 0;
                byte left = 1;
                byte down = 2;
                byte up = 3;
                int lastFoodTime,negativePoints,foodpoints;
                lastFoodTime = negativePoints = foodpoints = 0;
                int foodDissapearTime = 10000;
                int health = 3;
                int SoundPlayTime = 5;
                int WinScore = 100; //Score RequiredTo Win Game
                int GameHeightMax = 30;
                int GameHeightMin = 4;
                int userPoints = negativePoints;
                int direction = right;
                bool SoundCheck = false, pauseGame = false;
                string PlayerName, LineStatement;
                PlayerName = LineStatement = "";

                Random randomNumbersGenerator = new Random();

                //Snake directions from user - if any changes to this will only mess up the direction
                Position[] directions = new Position[]
                {
                    //direction speed
                    new Position( 0,  1),   // right
                    new Position( 0, -1),   // left
                    new Position( 1,  0),   // down
                    new Position(-1,  0),   // up
                };

                //Starts from the right side of terminal
                Console.BufferHeight = Console.WindowHeight;
                lastFoodTime = Environment.TickCount;

                //Creating Obstacles
                List<Position> obstacles = new List<Position>();
                for (int i = 0; i < 5; i ++)
                {
                    obstacles.Add (new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)));
                }

                //Creating Food
                List<Position> food = new List<Position>();
                for (int i = 0; i < 4; i ++)
                {
                    food.Add(new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)));
                }
                //Initialize Snake Length and Position
                Queue<Position> snakeElements = new Queue<Position>();
                for (int i = 0; i <= 3; i++)
                {
                    snakeElements.Enqueue(new Position(4, i));
                }

                List<string> startMenu = new List<string>() 
                {
                    "|      Start     |", "\n\n", 
                    "|   ScoreBoard   |", "\n\n",
                    "|   Mute Music   |", "\n\n",
                    "|      Exit      |"
                };

                List<string> PauseMenu = new List<string>()
                {
                    "|    Continue    |", "\n\n",
                    "|   ScoreBoard   |", "\n\n",
                    "|   Mute Music   |", "\n\n",
                    "|      Exit      |"
                };

                List<string> OverMenu = new List<string>()
                {
                    "|  Back To Menu  |", "\n\n",
                    "|   ScoreBoard   |", "\n\n",
                    "|   Mute Music   |", "\n\n",
                    "|      Exit      |"
                };

                Console.CursorVisible = false;
                Console.WindowHeight = 34;
                Console.WindowWidth = 125;

                PlayMusic("Background");
                MainMenu(startMenu, LineStatement, 0,0, false);

                PlayerName = NameScreen();
                DrawSnake(snakeElements);
                
                for (;;)
                { 
                    direction = InputCheck(up, down, left, right, direction, pauseGame, PauseMenu); 
                    DrawObstacles(obstacles);
                    Position snakeHead = snakeElements.Last();
                    Position nextDirection = directions[direction];
                    Position snakeNewHead = new Position(snakeHead.row + nextDirection.row, snakeHead.col + nextDirection.col);

                    //snake to move through terminal/program
                    if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 1;
                    if (snakeNewHead.row < GameHeightMin) snakeNewHead.row = GameHeightMax - 1;
                    if (snakeNewHead.row >= GameHeightMax) snakeNewHead.row = GameHeightMin;
                    if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;

                    //Score Board - Top Left
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    userPoints = negativePoints;
                    Console.WriteLine("Your Score  Points are: {0}\nYour Health Points are: {1}", userPoints, health);
                    Console.SetCursorPosition(57, 1);
                    Console.WriteLine("Snake Game");
                    Console.SetCursorPosition(110-PlayerName.Length, 1);
                    Console.WriteLine("Player Name: {0}", PlayerName);
                    Console.SetCursorPosition(0, 2);
                    Console.WriteLine("\n=============================================================================================================================");
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine("=============================================================================================================================");
                    Console.SetCursorPosition(45, 32);
                    Console.Write("[Press 'ENTER' To Pause The Game]");

                    //Winning Game
                    if (userPoints >= WinScore)
                    {
                        Console.SetCursorPosition(0, 2);
                        Console.ForegroundColor = ConsoleColor.Red;
                        userPoints = negativePoints;
                        Console.WriteLine("Congratulations you have reached 150 Points and Won the game but you can continue to play!");
                    }

                    //Game Over After Health = 0
                    if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                    {
                        Console.Clear();
                        PlayMusic("Damage");
                        SoundCheck = true;
                        health -= 1;
                        
                        if (health == 0)
                        {
                            PlayMusic("GameOver");
                            WriteFile(PlayerName, userPoints);      //Write Name and Score to text File   
                            
                            LineStatement = "You Scored " + userPoints + " points!";
                            MainMenu(OverMenu, LineStatement, userPoints, WinScore, true);      //shows the results after game ends and shows the back menu whether to quit, continue the game, check scoreboard or mute the music
                            break;
                        }
                    }
                    Console.SetCursorPosition(snakeHead.col, snakeHead.row);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("*");
                    
                    snakeElements.Enqueue(snakeNewHead);
                    Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (direction == right) Console.Write(">");
                    if (direction == left) Console.Write("<");
                    if (direction == up) Console.Write("^");
                    if (direction == down) Console.Write("v");

                    for (int i = 0; i < 4; i++)
                    {
                        if (snakeNewHead.col == food[i].col && snakeNewHead.row == food[i].row)
                        {
                            PlayMusic("Bite");
                            SoundCheck = true;
                            snakeElements.Enqueue(food[i]);
                            // feeding the snake
                            do
                            {
                                negativePoints = negativePoints + 1 + i;
                                food[i] = new Position(randomNumbersGenerator.Next(GameHeightMin, GameHeightMax), randomNumbersGenerator.Next(0, Console.WindowWidth));
                            }
                            while (snakeElements.Contains(food[i]) && obstacles.Contains(food[i]));
                            lastFoodTime = Environment.TickCount;
                            Console.SetCursorPosition(food[i].col, food[i].row);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(i + 1);
                            sleepTime--;

                            //add obstacle after eating food '1, 2, 3, 4'
                            Position obstaclecheck = new Position();
                            do
                            {
                                obstaclecheck = new Position(randomNumbersGenerator.Next(GameHeightMin, GameHeightMax), randomNumbersGenerator.Next(0, Console.WindowWidth));
                            }
                            while (snakeElements.Contains(obstaclecheck) || obstacles.Contains(obstaclecheck) && (food[i].row != obstaclecheck.row && food[i].col != obstaclecheck.col)); // && is the right code to prevent the blocks from staying the same line as food when random position
                            obstacles.Add(obstaclecheck);
                            DrawObstacles(obstacles);
                            break;
                        }
                    }

                    //Removing snake traces
                    Position last = snakeElements.Dequeue();
                    Console.SetCursorPosition(last.col, last.row);
                    Console.Write(" ");

                    //food traces then set at another location
                    if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Console.SetCursorPosition(food[i].col, food[i].row);
                            Console.Write(" ");

                            do
                            {
                                food[i] = new Position(randomNumbersGenerator.Next(GameHeightMin, GameHeightMax), randomNumbersGenerator.Next(0, Console.WindowWidth));
                            }
                            while (snakeElements.Contains(food[i]) || obstacles.Contains(food[i]));
                            lastFoodTime = Environment.TickCount;
                            foodpoints++;
                        }
                    }                  

                    DrawFood(food);
                    sleepTime -= 0.01;
                    Thread.Sleep((int)sleepTime);

                    if (SoundCheck == true)
                    {
                        if (SoundPlayTime == 0)
                        {
                            PlayMusic("Background");
                            SoundPlayTime = 2;
                            SoundCheck = false;
                        }
                        else
                        {
                            SoundPlayTime--;
                        }
                    }
                } 
            }
        }
    }
}