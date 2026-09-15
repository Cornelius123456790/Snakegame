using System;
using System.Collections.Generic;
using System.Threading;

class SnakeGame
{
    static int width = 40;
    static int height = 20;
    static int score = 0;
    static int speed = 150;

    static int[] headX = { 10 };
    static int[] headY = { 10 };
    static List<(int X, int Y)> snake = new List<(int, int)>();

    static int foodX;
    static int foodY;
    static Random rnd = new Random();

    static string direction = "RIGHT";
    static bool gameOver = false;

    static void Main()
    {
        Console.CursorVisible = false;
        Console.Title = "Змейка на C#";

        snake.Add((10, 10));
        snake.Add((9, 10));
        snake.Add((8, 10));

        SpawnFood();

        // Поток для чтения нажатий клавиш
        Thread inputThread = new Thread(ReadInput);
        inputThread.IsBackground = true;
        inputThread.Start();

        while (!gameOver)
        {
            Draw();
            Move();
            Thread.Sleep(speed);
        }

        Console.Clear();
        Console.SetCursorPosition(width / 2 - 5, height / 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Игра окончена! Очки: {score}");
        Console.ResetColor();
        Console.SetCursorPosition(0, height + 2);
        Console.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static void ReadInput()
    {
        while (!gameOver)
        {
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (direction != "DOWN") direction = "UP";
                    break;
                case ConsoleKey.DownArrow:
                    if (direction != "UP") direction = "DOWN";
                    break;
                case ConsoleKey.LeftArrow:
                    if (direction != "RIGHT") direction = "LEFT";
                    break;
                case ConsoleKey.RightArrow:
                    if (direction != "LEFT") direction = "RIGHT";
                    break;
            }
        }
    }

    static void Move()
    {
        var head = snake[0];
        (int X, int Y) newHead = direction switch
        {
            "UP" => (head.X, head.Y - 1),
            "DOWN" => (head.X, head.Y + 1),
            "LEFT" => (head.X - 1, head.Y),
            "RIGHT" => (head.X + 1, head.Y),
            _ => head
        };

        // Столкновение со стеной
        if (newHead.X < 0 || newHead.X >= width || newHead.Y < 0 || newHead.Y >= height)
        {
            gameOver = true;
            return;
        }

        // Столкновение с собой
        if (snake.Contains(newHead))
        {
            gameOver = true;
            return;
        }

        snake.Insert(0, newHead);

        // Съели еду
        if (newHead.X == foodX && newHead.Y == foodY)
        {
            score += 10;
            speed = Math.Max(50, speed - 5); // ускорение
            SpawnFood();
        }
        else
        {
            snake.RemoveAt(snake.Count - 1); // убираем хвост
        }
    }

    static void SpawnFood()
    {
        bool valid = false;
        while (!valid)
        {
            foodX = rnd.Next(0, width);
            foodY = rnd.Next(0, height);
            valid = !snake.Contains((foodX, foodY));
        }
    }

    static void Draw()
    {
        Console.SetCursorPosition(0, 0);

        // Верхняя граница
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(new string('#', width + 2));

        for (int y = 0; y < height; y++)
        {
            Console.Write("#");
            for (int x = 0; x < width; x++)
            {
                var pos = (x, y);
                if (pos == snake[0])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("@");
                }
                else if (snake.Contains(pos))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("o");
                }
                else if (x == foodX && y == foodY)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("*");
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(" ");
                }
            }
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("#");
        }

        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(new string('#', width + 2));
        Console.ResetColor();

        Console.WriteLine($"Очки: {score}  |  Скорость: {speed}ms  |  Управление: стрелки");
    }
}
