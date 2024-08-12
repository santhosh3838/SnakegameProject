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
            Console.CursorVisible = false;
            Console.SetWindowSize(width + 2, height + 2); // Adding space for borders
            Console.SetBufferSize(width + 2, height + 2); // Adding space for borders

            do
            {
                Console.Clear(); // Clear screen before starting new game
                InitializeGame();

                while (!gameOver)
                {
                    HandleInput();
                    UpdateGame();
                    Draw_border_and_snake();
                    Thread.Sleep(100); // Adjust speed as needed
                }

                // Display game over message and ask if the user wants to play again
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(width / 2 - 10, height / 2);
                Console.WriteLine($"Game Over! Your Score: {snakeSize}");
                Console.ResetColor();

                Console.SetCursorPosition(width / 2 - 10, height / 2 + 1);
                Console.Write("Play again? (Y/N): ");
            }
            //trim remove whitespaces and toupper converts to uppercase and evaluates it &returns true or false
            //or u can write like this also   while (Console.ReadLine() == "Y");
            while (Console.ReadLine()?.Trim().ToUpper() == "Y");



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

        static void UpdateGame()
        {
            //if (gameOver) return;

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
            }
            else
            {
                // Remove the tail segment if not eating food
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
                food = new Food(random.Next(0, width), random.Next(0, height));
            }
        }

        static void Draw_border_and_snake()
        {
            Console.Clear();

            // Draw the border
            Console.ForegroundColor = ConsoleColor.White;
            for (int x = 0; x <= width + 1; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write("■");
                Console.SetCursorPosition(x, height + 1);
                Console.Write("■");
            }
            for (int y = 0; y <= height + 1; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write("■");
                Console.SetCursorPosition(width + 1, y);
                Console.Write("■");
            }
            Console.ResetColor();

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
            Console.Write("●");
            Console.ResetColor();

            // Display the current score
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(1, height + 1); // Score displayed below the grid
            Console.Write($"Score: {snakeSize}");
            Console.ResetColor();
        }
    }
}

