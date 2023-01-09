namespace TicTacToeAI;
public class Game
{
    public static int MapSize = 10;
    public static int WinCount = 5;
    public static int Depth = 5;
    static bool SomeoneWins = false;
    static int[] players = { 1, -1 };
    static bool AIFirstMove = true;
    static bool PlayerStarted { get; set; }
    static int PlayerStartX { get; set; }
    static int PlayerStartY { get; set; }

    public static void Main(string[] args)
    {
        Tests.AITests();
        return;
        TicTacToe();
    }

    static void TicTacToe()
    {

        int[,] map = new int[MapSize, MapSize];
        map = GenerateMap(map);
        var random = new Random();
        bool playerPlays = random.Next(0, 100) > 50;
        PlayerStarted = playerPlays;
        PlayerStarted = false;
        do
        {
            if (playerPlays)
            {
                //Console.Clear();
                DrawMap(map);
                var valueX = Console.ReadLine();
                var valueY = Console.ReadLine();
                var posX = valueX == null ? -1 : int.Parse(valueX);
                var posY = valueY == null ? -1 : int.Parse(valueY);
                if (map[posX, posY] != 0 || posX == -1 || posY == -1) continue;
                PlayerStartX = posX;
                PlayerStartY = posY;
                map[posX, posY] = 1;
                if (CheckWin(map, out var player))
                {
                    SomeoneWins = true;
                    continue;
                }
                if (!SomethingToPlay(map))
                {
                    //Console.Clear();
                    DrawMap(map);
                    Console.WriteLine("DRAW");
                    return;
                }
                playerPlays = false;
            }
            else
            {
                if (AIFirstMove)
                {
                    if (!PlayerStarted)
                    {
                        var X = random.Next((MapSize / 2) - 1, (MapSize / 2) + 1);
                        var Y = random.Next((MapSize / 2) - 1, (MapSize / 2) + 1);
                        map[X, Y] = -1;
                        AIFirstMove = false;
                        playerPlays = true;
                        continue;
                    }
                    var x = PlayerStartX + random.Next(-1, 1);
                    var y = PlayerStartY + random.Next(-1, 1);
                    map[x, y] = -1;
                    AIFirstMove = false;
                    playerPlays = true;
                    continue;
                }

                AI.GetAIMove(map);
                if (CheckWin(map, out var player))
                {
                    SomeoneWins = true;
                    continue;
                }
                if (!SomethingToPlay(map))
                {
                    //Console.Clear();
                    DrawMap(map);
                    Console.WriteLine("DRAW");
                    return;
                }
                AIFirstMove = false;
                playerPlays = true;
            }
        } while (!SomeoneWins);
        DrawMap(map);
        var winner = playerPlays == true ? "Player" : "AI";
        Console.WriteLine(winner);
    }

    static int[,] GenerateMap(int[,] map)
    {
        for (int y = 0; y < MapSize; y++) for (int x = 0; x < MapSize; x++) map[y, x] = 0;
        return map;
    }

    public static void DrawMap(int[,] map)
    {
        for (int y = 0; y < MapSize; y++)
        {
            for (int i = 0; i < MapSize; i++)
            {
                if (map[y, i] != 0)
                {
                    var customize = map[y, i] == 1 ? "X" : "O";
                    var color = customize == "X" ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.ForegroundColor = color;
                    Console.Write($"{customize}    ");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
                Console.Write($"{y}{i}   ");
            }
            Console.WriteLine();
        }
    }    

    public static bool CheckWin(int[,] map, out int player)
    {
        player = 0;

        for (int p = 0; p < 2; p++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                for (int x = 0; x < MapSize; x++)
                {
                    // check for vertical
                    if (x + WinCount <= MapSize)
                    {
                        var win = true;
                        for (int i = 0; i < WinCount; i++)
                        {
                            if (map[x + i,y] != players[p])
                            {
                                win = false;
                            }
                        }
                        if (win == true)
                        {
                            player = players[p];
                            return true;
                        }
                    }
                    // check for horizontal
                    if (y + WinCount <= MapSize)
                    {
                        var win = true;
                        for (int i = 0; i < WinCount; i++)
                        {
                            if (map[x,y + i] != players[p])
                            {
                                win = false;
                            }
                        }
                        if (win == true)
                        {
                            player = players[p];
                            return true;
                        }
                    }
                    // check for diagonal win
                    if (x + WinCount < MapSize + 1 && y + WinCount - 1 < MapSize)
                    {
                        var win = true;
                        for (int i = 0; i < WinCount; i++)
                        {
                            if (map[x + i,y + i] != players[p])
                            {
                                win = false;
                            }
                        }
                        if (win == true)
                        {
                            player = players[p];
                            return true;
                        }

                    }
                    // check for anti diagonal
                    if (x + WinCount < MapSize + 1 && y - WinCount >= -1)
                    {
                        var win = true;
                        for (int i = 0; i < WinCount; i++)
                        {
                            if (map[x + i,y - i] != players[p])
                            {
                                win = false;
                            }
                        }
                        if (win == true)
                        {
                            player = players[p];
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public static bool SomethingToPlay(int[,] map)
    {
        for (int y = 0; y < MapSize; y++) for (int x = 0; x < MapSize; x++) if(map[x, y] == 0) return true;
        return false;
    }
}