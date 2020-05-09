using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Media;
using System.IO;
using System.Text.RegularExpressions;

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
        static int InputCheck(byte up, byte down, byte left, byte right, int direction,bool pause)
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
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
                }else if (pause == false)
                {
                    int numTimes = 0;
                    if(userInput.Key == ConsoleKey.Spacebar && numTimes == 0)
                    {
                        pause = true;
                        Console.SetCursorPosition(36,12);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("   \n\t\t\t\t    --------------------------------------------------   \n\t\t\t\t    ||||||||||||||||||| GAME PAUSE |||||||||||||||||||   \n\t\t\t\t    ||||||| Please press 'SPACEBAR' to Continue ||||||   \n\t\t\t\t    || Else it will return to the game in 1 minute |||   \n\t\t\t\t    --------------------------------------------------   "); 
                        Thread.Sleep(30000);
                        Console.Clear();
                        numTimes+=1;
                        if (userInput.Key != ConsoleKey.Spacebar && numTimes == 1)
                        {
                            return 0;
                            numTimes -=1;
                            
                        }
                    }
                }
            } return direction;
        }
       
        //Method Displays Name Request Screen
        static string NameScreen()
        {
            string PlayerName;
            Console.Clear();
            Console.SetCursorPosition(43, 16);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please Enter Your Name and Press Enter");
            Console.SetCursorPosition(59, 17);
            PlayerName = Console.ReadLine();
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
                Console.SetCursorPosition(40, i+3);
                Console.Write(Name[i]);
                Console.SetCursorPosition(70, i+3);
                Console.Write(Score[i]);
            }
            Console.SetCursorPosition(0, 0);
            Console.Write("Press The Escape Key to Return to Main Menu");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
            //MainMenu();
        }

        //Displays Main Menu
        static void MainMenu(List<string> menuOpts) 
        { 
           int index = 0;
            //List print out options for user to select by pressing up and down keys
            
            while(true)
            {  
                for (int i = 0; i < menuOpts.Count; i++)
                {   
                    Console.SetCursorPosition(55,13 + i); 
                     if (i == index)
                     {
                        //selecton background colour
                        Console.BackgroundColor = ConsoleColor.Green;
                        //foreground
                        Console.ForegroundColor = ConsoleColor.Black; 
                    }else
                    {
                        Console.ResetColor();
                    }
                    Console.WriteLine(menuOpts[i]);//prints out the list
            }
                //obtains the next character or any key pressed by the user.  
                ConsoleKeyInfo presskey = Console.ReadKey();
            
                //moves selection down by index
                if (presskey.Key == ConsoleKey.DownArrow)
            {
                if (index == menuOpts.Count-1)
                {index = 0;
                 Console.BackgroundColor = ConsoleColor.Black;
                }else { index += 2; }

                //moves selection up by index
            }else if (presskey.Key == ConsoleKey.UpArrow)
                {if (index <= 0)
                    {index = menuOpts.Count-1; 
                    }else { index -= 2;Console.BackgroundColor = ConsoleColor.Black;}
                    
            }else if (presskey.Key == ConsoleKey.Enter )
                {//Start to proceed to the game
                   if (index == 0)
                    {Console.Clear();
                     return;
                    }

                   //displays scoreboard of each player
                   else if (index == 2)
                    {DisplayScoreBoard();}

                   //select 'Exit' to close the program
                   else if (index == 4)
                    {Environment.Exit(0);}
                }
                Console.Clear();
            }
        }

        static void Main(string[] args)
        {
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
                int SoundPlayTime = 5;              //Score RequiredTo Win Game
                int GameHeightMax = 30;
                int GameHeightMin = 4;
                int userPoints = negativePoints;
                int direction = right;
                bool SoundCheck,pauseGame;
                SoundCheck = pauseGame= false;
                string PlayerName = "";

                Random randomNumbersGenerator = new Random();
               
                var BiteSound = new SoundPlayer(); BiteSound.SoundLocation = @"Sounds\Bite.wav";
                var DamageSound = new SoundPlayer(); DamageSound.SoundLocation = @"Sounds\Damage.wav";
                var GameOverSound = new SoundPlayer(); GameOverSound.SoundLocation = @"Sounds\GameOver.wav";
                var BackgroundSound = new SoundPlayer(); BackgroundSound.SoundLocation = @"Sounds\Background.wav";

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
                List<Position> obstacles = new List<Position>()
                {
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)),
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)),
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)),
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)),
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth))
                };

                //Creating Food
                List<Position> food = new List<Position>()
                {
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)),
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)),
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth)),
                    new Position(randomNumbersGenerator.Next(GameHeightMin,GameHeightMax), randomNumbersGenerator.Next(0,Console.WindowWidth))
                };

                //Initialize Snake Length and Position
                Queue<Position> snakeElements = new Queue<Position>();
                for (int i = 0; i <= 3; i++)
                {
                    snakeElements.Enqueue(new Position(4, i));
                }

                List<string> startMenu = new List<string>() {
                    "|      Start     |", "\n\n", 
                    "|   ScoreBoard   |", "\n\n",
                    "|      Exit      |"
                };
                Console.CursorVisible = false;
                Console.WindowHeight = 34;
                Console.WindowWidth = 125;
                BackgroundSound.PlayLooping();
                MainMenu(startMenu);
                PlayerName = NameScreen();
                DrawSnake(snakeElements);
                
                for (; ;)
                { 
                    direction = InputCheck(up, down, left, right, direction,pauseGame); 
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
                    Console.WriteLine("Your Score  Points are: {0} \t\t\t\t Snake Game \t\t [Press 'SPACEBAR' key to Pause the game.] \nYour Health Points are: {1}\t\t\t\t Player: {2}", userPoints, health, PlayerName);
                    Console.WriteLine("\n=============================================================================================================================");
                    Console.SetCursorPosition(0, 30);
                    Console.WriteLine("=============================================================================================================================");
             
                    //Winning Game
                    if (userPoints >= 150)
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
                        //DrawObstacles(obstacles);
                        DamageSound.Play();
                        SoundCheck = true;
                        health -= 1;
                        
                        if (health == 0)
                        {
                            List<string> OverMenu = new List<string>() {
                                                    "|  Back To Menu  |", "\n\n", 
                                                    "|   ScoreBoard   |", "\n\n",
                                                    "|      Exit      |"
                            };
                            GameOverSound.Play();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Clear();
                            Console.SetCursorPosition(59,10);
                            if (userPoints >= 150)
                            {
                                Console.WriteLine("You Won!");
                            }
                            else
                            {
                                Console.WriteLine("Game over!");
                            }
                            
                            foreach (string i in OverMenu)
                            {
                                Console.SetCursorPosition(54, 11);
                                Console.WriteLine("Your Scored {0} points", userPoints);
                                MainMenu(OverMenu);
                            }
                            Console.SetCursorPosition(50, 14);
                            WriteFile(PlayerName, userPoints);      //Write Name and Score to text File
                            //Console.Write("Press The Enter Key to Exit");
                            //while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                           
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
                            BiteSound.Play();
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
                            BackgroundSound.PlayLooping();
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