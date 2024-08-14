using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame
{

    partial class Program
    {
        static Direction currentDirection = Direction.Right;
        static List<SnakeSegment> SnakeBody = new List<SnakeSegment>();//empty list to store snakebody  
        static Food food;// declares a field that will store the Food object repersents position of the food =new Food(x,y) recieves two prmtrs in generate food method 
        static int snakeSize = 1;
        static int width = 50, height = 30;
        static bool gameOver = false;

        static void Main()
        {
            Console.SetCursorPosition(35, 2);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Do You want to start the game(Y/N)");
            Console.SetCursorPosition(40, 3);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Uparrow to move up");
            Console.SetCursorPosition(40, 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Downarrow to move Down");
            Console.SetCursorPosition(40, 5);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Leftarrow to move Left");
            Console.SetCursorPosition(40, 6);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Rightarrow to move Rigtht");
            Console.ResetColor();
            while (Console.ReadLine()?.Trim().ToUpper() == "Y")
            {
                Console.CursorVisible = false;
                //Console.SetWindowSize(width + 2, height + 2); // Adding space for borders
                //Console.SetBufferSize(width + 2, height + 2); // Adding space for borders


                do
                {
                    Draw_border();
                    // Clear screen before starting new game
                    InitializeGame();
                    int snakespeed = 150;

                    DateTime StartTime = DateTime.Now;//this is to note the current time 

                    while (!gameOver)
                    {

                        HandleInput();

                        UpdateGame(ref snakespeed);

                        draw_snake();
                        //if ((DateTime.Now - StartTime).TotalSeconds >= 5)
                        //{
                        //    delay =Math.Max(50, delay-20);
                        //}

                        Thread.Sleep(snakespeed); // Adjust speed as needed
                                                  // StartTime = DateTime.Now;

                    }
                    TimeSpan TimeTaken_to_complete_game = DateTime.Now - StartTime;
                    // Display game over message and ask if the user wants to play again
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.SetCursorPosition(width / 2 - 10, height / 2);
                    Console.WriteLine($"Game Over! Your Score: {snakeSize}");
                    Console.SetCursorPosition(width / 2 - 10, height / 2 + 1);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Time Taken to Complete the game: {TimeTaken_to_complete_game.Minutes}m {TimeTaken_to_complete_game.Seconds}s");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(width / 2 - 10, height / 2 + 2);
                    Console.Write("Play again? (Y/N): ");
                }
                //trim remove whitespaces and toupper converts to uppercase and evaluates it &returns true or false
                //or u can write like this also   while (Console.ReadLine() == "Y");
                while (Console.ReadLine()?.Trim().ToUpper() == "Y");


            }
        }

        static void InitializeGame()
        {
            SnakeBody.Clear();
            SnakeBody.Add(new SnakeSegment(width / 2, height / 2)); // Snake starts at the center
            snakeSize = 1;
            gameOver = false;
            GenerateFood();
        }

        static void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentDirection != Direction.Down) currentDirection = Direction.Up;
                        // Changes direction to Up if the current direction is not Down
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentDirection != Direction.Up) currentDirection = Direction.Down;
                        // Changes direction to down if the current direction is not up
                        break;
                    case ConsoleKey.LeftArrow:
                        if (currentDirection != Direction.Right) currentDirection = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (currentDirection != Direction.Left) currentDirection = Direction.Right;
                        break;
                }
            }
        }

        static void UpdateGame(ref int snakespeed)
        {
            //if (gameOver) return;

            int lastSpeedIncreaseSize = 0;

            var head = SnakeBody.First();
            int newpositionX = head.X;
            int newpositionY = head.Y;

            switch (currentDirection)
            {
                case Direction.Up:
                    newpositionY--;
                    break;
                case Direction.Down:
                    newpositionY++;
                    break;
                case Direction.Left:
                    newpositionX--;
                    break;
                case Direction.Right:
                    newpositionX++;
                    break;
            }

            // Check if the snake collides with the walls
            if (newpositionX < 0 || newpositionX >= width || newpositionY < 0 || newpositionY >= height)
            {
                gameOver = true;
                return;
            }

            // Check if the snake collides with itself
            if (SnakeBody.Any(segment => segment.X == newpositionX && segment.Y == newpositionY))
            {
                gameOver = true;
                return;

            }

            // checks if the snake's new position matches the food’s position to determine if the snake has eaten the food
            if (newpositionX == food.X && newpositionY == food.Y)
            {
                snakeSize++; // Increase size
                GenerateFood(); // Generate new food
                                //if(snakeSize == 2)
                                //{
                                //    delay = Math.Min(50, delay - 120);
                                //}


                //int nextSpeedIncreaseSize = ((snakeSize / 5) + 1) * 5;//in multiples for odd


                int nextSpeedIncreaseSize = (lastSpeedIncreaseSize + 2);
                if (snakeSize >= nextSpeedIncreaseSize && snakeSize > lastSpeedIncreaseSize)
                {
                    snakespeed = Math.Max(50, snakespeed - 30); // Increase speed
                    lastSpeedIncreaseSize = nextSpeedIncreaseSize; // Update last size where speed was increased
                }
            }
            else
            {
                // Remove the tail segment if not eating food
                var tail = SnakeBody.Last();
                Console.SetCursorPosition(tail.X + 1, tail.Y + 1);
                Console.Write(" ");
                SnakeBody.RemoveAt(SnakeBody.Count - 1);
            }

            // Add the new head position
            SnakeBody.Insert(0, new SnakeSegment(newpositionX, newpositionY));

        }

        static void GenerateFood()
        {
            var random = new Random();//

            food = new Food(random.Next(0, width), random.Next(0, height));

            // Ensure food does not appear on the snake
            while (SnakeBody.Any(segment => segment.X == food.X && segment.Y == food.Y))
            {
                Console.CursorVisible = false;
                food = new Food(random.Next(0, width), random.Next(0, height));
            }
        }

        static void Draw_border()
        {
            Console.Clear();

            // Draw the border
            Console.ForegroundColor = ConsoleColor.White;
            for (int x = 2; x <= width; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write("■");
                Console.SetCursorPosition(x, height + 2);
                Console.Write("■");
            }
            for (int y = 1; y <= height + 1; y++)
            {
                Console.SetCursorPosition(1, y);
                Console.WriteLine("▐");
                Console.SetCursorPosition(width, y);
                Console.Write("▐");
            }
            Console.ResetColor();



        }
        static void draw_snake()
        {
            // Draw the snake
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var segment in SnakeBody)
            {
                Console.SetCursorPosition(segment.X + 1, segment.Y + 1); // Adjust for border
                Console.Write("■");
            }

            // Draw the food
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(food.X + 1, food.Y + 1); // Adjust for border
            Console.Write("#");
            Console.ResetColor();




            // Display the current score
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(width + 4, 0); // Score displayed below the grid
            Console.Write($"Score: {snakeSize}");

            Console.ResetColor();



        }
    }
}

