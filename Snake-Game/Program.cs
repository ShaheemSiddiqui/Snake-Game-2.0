using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Media;
using System.IO;

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
        static void Main(string[] args)
        {
            double  sleepTime           = 100;              //SleepTime indicates speed movement of the snake, the higher the number, the slower the speed of the snake
            byte    right               = 0;
            byte    left                = 1;
            byte    down                = 2;
            byte    up                  = 3;
            int     lastFoodTime        = 0;
            int     foodDissapearTime   = 10000;
            int     health              = 3;
            int     negativePoints      = 0;
            int     SoundPlayTime       = 5;
            int     WinningScore        = 150;              //Score RequiredTo Win Game
            int     GameHeightMax       = 31;
            int     GameHeightMin       = 4;
            int     foodpoints          = 0;
            int     userPoints          = negativePoints;
            int     direction           = right;
            bool    SoundCheck          = false;
            string  PlayerName          = "";

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

            Console.CursorVisible = false;
            Console.WindowHeight = 34;
            Console.WindowWidth = 125;
            BackgroundSound.PlayLooping();
            PlayerName = NameScreen();
            DrawObstacles(obstacles);
            DrawSnake(snakeElements);

            for(;;)
            {
                direction = DirectionCheck(up, down, left, right, direction);
                
                Position snakeHead = snakeElements.Last();
                Position nextDirection = directions[direction];
                Position snakeNewHead = new Position(snakeHead.row + nextDirection.row, snakeHead.col + nextDirection.col);

                //snake to move through terminal/program
                if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 1;
                if (snakeNewHead.row < GameHeightMin) snakeNewHead.row = GameHeightMax-1;
                if (snakeNewHead.row >= GameHeightMax) snakeNewHead.row = GameHeightMin;
                if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;

                //Score Board - Top Left
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Red;
                userPoints = negativePoints;
                Console.WriteLine("Your Score  Points are: {0} \t\t\t\t Snake Game \t\t\t Player: {1}\nYour Health Points are: {2}\n", userPoints, PlayerName, health);
                Console.WriteLine("=============================================================================================================================");
                Console.SetCursorPosition(0, 31);
                Console.WriteLine("=============================================================================================================================");


                //Winning Game
                if (userPoints >= WinningScore)
                {
                    Console.SetCursorPosition(0, 2);
                    Console.ForegroundColor = ConsoleColor.Red;
                    userPoints = negativePoints;
                    Console.WriteLine("Congratulations you have reached {0} Points and Won the game but you can continue to play!", WinningScore);
                }

                //Game Over After Health = 0
                if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                {
                    DamageSound.Play();
                    SoundCheck = true;
                    Console.Clear();
                    health -= 1;
                    DrawObstacles(obstacles);

                    if (health == 0)
                    {
                        GameOverSound.Play();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Clear();
                        Console.SetCursorPosition(55, 10);
                        if (userPoints >= WinningScore) 
                        { 
                            Console.WriteLine("You Won!");
                        }
                        else 
                        { 
                            Console.WriteLine("Game over!"); 
                        }
                        Console.SetCursorPosition(51, 12);
                        Console.WriteLine("Your Scored {0} points", userPoints);
                        Console.SetCursorPosition(45, 14);
                        WriteFile(PlayerName, userPoints);      //Write Name and Score to text File
                        Console.Write("Press The Enter Key to Exit");
                        while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                        return;
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
                        Console.Write(i+1);
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
        static int DirectionCheck(byte up, byte down, byte left, byte right, int direction)
        {
            if (Console.KeyAvailable)
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
                }
            }
            return direction;
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

        //Method Outputs Name & Score to Text File
        static void WriteFile(string PlayerName, int userPoints)
        {
            string FileName = @"Records\records.txt";
            File.AppendAllText(FileName, PlayerName + "\t" + userPoints + Environment.NewLine);
        }
    }
}