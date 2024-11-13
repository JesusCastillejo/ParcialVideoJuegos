using System;
using System.Media;
using Tao.Sdl;

namespace MyGame
{
    class Program
    {
        static Image backgroundImage = Engine.LoadImage("assets/Board.jpg");
        static Image paddleImage = Engine.LoadImage("assets/Paddle.jpg");
        static Image ballImage = Engine.LoadImage("assets/Ball.png");
        static Image victoryImage = Engine.LoadImage("assets/Victory.jpg");
        static Image defeatImage = Engine.LoadImage("assets/YouLose.jpg");
        static Image starScreenImage = Engine.LoadImage("assets/Start.jpg");
        static Font Fuente;

        static SoundPlayer Music;

        static int paddleWidth = 20, paddleHeight = 100;
        static int ballWidth = 20, ballHeight = 20;

        static int player1Score = 0, player2Score = 0;
        static int maxScore = 10;

        static bool gameRunning = false;
        static bool isMultiplayer = false;

        static int ballX, ballY, ballSpeedX = 5, ballSpeedY = 5;
        static int hits = 0;

        static string winner = "";

        struct Paddle
        {
            public int X, Y, Speed;
            public Paddle(int x, int y, int speed)
            {
                X = x;
                Y = y;
                Speed = speed;
            }
        }

        static Paddle player1;
        static Paddle player2;

        static Random random = new Random();

        static void Main(string[] args)
        {
            Engine.Initialize();

            Music = new SoundPlayer("assets/happyToBe.wav");
            Music.PlayLooping();

            Fuente = Engine.LoadFont("assets/DotoVideoGame.ttf", 30);

            ShowStartScreen();

            while (true)
            {
                if (gameRunning)
                {
                    CheckInputs();
                    Update();
                    Render();
                }
                else
                {
                    ShowStartScreen();
                }
                Sdl.SDL_Delay(20);
            }
        }

        static void ShowStartScreen()
        {
            Engine.Clear();
            Engine.Draw(starScreenImage, 0, 0);
            Engine.DrawText("Presione 1 para un jugador", 100, 100, 255, 255, 255, Fuente);
            Engine.DrawText("Presione 2 para dos jugadores", 100, 150, 255, 255, 255, Fuente);
            Engine.DrawText("Presione ESC para salir", 100, 200, 255, 255, 255, Fuente);
            Engine.Show();

            while (!Engine.KeyPress(Engine.KEY_1) && !Engine.KeyPress(Engine.KEY_2) && !Engine.KeyPress(Engine.KEY_ESC))
            {
                Sdl.SDL_Delay(20); 
            }

            if (Engine.KeyPress(Engine.KEY_1))
            {
                StartGame(false);
            }
            else if (Engine.KeyPress(Engine.KEY_2))
            {
                StartGame(true);
            }
            else if (Engine.KeyPress(Engine.KEY_ESC))
            {
                Environment.Exit(0);
            }
        }

        static void StartGame(bool multiplayer)
        {
            isMultiplayer = multiplayer;
            gameRunning = true;
            player1Score = 0;
            player2Score = 0;
            ballX = 512; ballY = 384;
            ballSpeedX = 5; ballSpeedY = 5;
            hits = 0;
            winner = "";

            player1 = new Paddle(50, 334, 10);
            player2 = new Paddle(954, 334, 10);
        }

        static void CheckInputs()
        {
            if (isMultiplayer)
            {
                if (Engine.KeyPress(Engine.KEY_W))
                {
                    player1.Y -= player1.Speed;
                    if (player1.Y < 0) player1.Y = 0;
                }
                if (Engine.KeyPress(Engine.KEY_S))
                {
                    player1.Y += player1.Speed;
                    if (player1.Y > 768 - paddleHeight) player1.Y = 768 - paddleHeight;
                }

                if (Engine.KeyPress(Engine.KEY_UP))
                {
                    player2.Y -= player2.Speed;
                    if (player2.Y < 0) player2.Y = 0;
                }
                if (Engine.KeyPress(Engine.KEY_DOWN))
                {
                    player2.Y += player2.Speed;
                    if (player2.Y > 768 - paddleHeight) player2.Y = 768 - paddleHeight;
                }
            }
            else
            {
                if (Engine.KeyPress(Engine.KEY_W))
                {
                    player1.Y -= player1.Speed;
                    if (player1.Y < 0) player1.Y = 0;
                }
                if (Engine.KeyPress(Engine.KEY_S))
                {
                    player1.Y += player1.Speed;
                    if (player1.Y > 768 - paddleHeight) player1.Y = 768 - paddleHeight;
                }

                if (random.NextDouble() < 0.55)
                {
                    if (ballY > player2.Y + paddleHeight / 2)
                    {
                        player2.Y += player2.Speed - random.Next(0, 3);
                    }
                    else
                    {
                        player2.Y -= player2.Speed - random.Next(0, 3);
                    }
                }
                if (player2.Y < 0) player2.Y = 0;
                if (player2.Y > 768 - paddleHeight) player2.Y = 768 - paddleHeight;
            }

            if (Engine.KeyPress(Engine.KEY_ESC))
            {
                Environment.Exit(0);
            }
        }

        static void Update()
        {
            ballX += ballSpeedX;
            ballY += ballSpeedY;

            if (ballY < 0 || ballY + ballHeight > 768)
            {
                ballSpeedY = -ballSpeedY;
            }

            if (ballX < 0)
            {
                player2Score++;
                ResetBall();
                if (player2Score >= maxScore)
                {
                    winner = isMultiplayer ? "Jugador 2" : "Computadora";
                    ShowEndScreen();
                }
            }

            if (ballX + ballWidth > 1024)
            {
                player1Score++;
                ResetBall();
                if (player1Score >= maxScore)
                {
                    winner = "Jugador 1";
                    ShowEndScreen();
                }
            }

            if (CheckCollision(ballX, ballY, ballWidth, ballHeight, player1.X, player1.Y, paddleWidth, paddleHeight) ||
                CheckCollision(ballX, ballY, ballWidth, ballHeight, player2.X, player2.Y, paddleWidth, paddleHeight))
            {
                ballSpeedX = -ballSpeedX;
                hits++;
                if (hits % 5 == 0)
                {
                    if (ballSpeedX > 0) ballSpeedX++;
                    else ballSpeedX--;
                    if (ballSpeedY > 0) ballSpeedY++;
                    else ballSpeedY--;
                }
            }
        }

        static bool CheckCollision(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
        {
            return x1 < x2 + w2 && x1 + w1 > x2 && y1 < y2 + h2 && y1 + h1 > y2;
        }

        static void ResetBall()
        {
            ballX = 512;
            ballY = 384;
            ballSpeedX = 5;
            ballSpeedY = 5;
            hits = 0;
        }

        static void ShowEndScreen()
        {
            gameRunning = false;
            Engine.Clear();

            if( isMultiplayer)
            {
                Engine.Draw(victoryImage, 0, 0);
                if(!string.IsNullOrEmpty(winner))
                {
                    Engine.DrawText($"¡{winner} gana!", 370, 360, 255, 255, 255, Fuente);
                }
            }
            else
            {
                if(winner == "jugador 1")
                {
                    Engine.Draw(victoryImage, 0, 0);
                }
                else
                {
                    Engine.Draw(defeatImage, 0, 0);
                }
            }
            Engine.Show();
            Sdl.SDL_Delay(5000);
        }

        static void Render()
        {
            Engine.Clear();
            Engine.Draw(backgroundImage, 0, 0);
            Engine.Draw(paddleImage, player1.X, player1.Y);
            Engine.Draw(paddleImage, player2.X, player2.Y);
            Engine.Draw(ballImage, ballX, ballY);

            Engine.DrawText($"Jugador 1: {player1Score}", 10, 10, 255, 255, 255, Fuente);
            Engine.DrawText($"Jugador 2: {player2Score}", 790, 10, 255, 255, 255, Fuente);

            Engine.Show();
        }
    }
}
