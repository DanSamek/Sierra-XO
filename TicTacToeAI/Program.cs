namespace TicTacToeAI;
public class Program
{
    static int MapSize = 7;
    static bool SomeoneWins = false;
    static int WinCount = 5;
    static int[] players = { 1, -1 };
    static bool AIFirstMove = true;
    static bool PlayerStarted { get; set; }
    static int PlayerStartX { get; set; }
    static int PlayerStartY { get; set; }

    // TODO 2)Multithreading +  1)TransitionTables => fast depth 5 and more
    public static void Main(string[] args)
    {
        int[,] map = new int[MapSize, MapSize];
        map = GenerateMap(map);
        var random = new Random();
        bool playerPlays = random.Next(0, 100) > 50;
        PlayerStarted = playerPlays;
        do
        {
            if (playerPlays)
            {
                DrawMap(map);
                var pos = Console.ReadLine();
                if (!int.TryParse(pos[0].ToString(), out int x)) continue;
                if (!int.TryParse(pos[1].ToString(), out int y)) continue;
				if(map[x, y] != 0) continue;
                PlayerStartX = x;
                PlayerStartY = y;
                map[x, y] = 1;
                if (CheckWin(map, out var player))
                {    
                    SomeoneWins = true;
                    continue;   
                }
                if (!SomethingToPlay(map))
                {
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
                        var X = random.Next((MapSize / 2) -1, (MapSize / 2) + 1);
                        var Y = random.Next((MapSize / 2)-1, (MapSize / 2) + 1);
                        map[X, Y] = -1;
                        AIFirstMove = false;
                        playerPlays = true;
                        continue;
                    }
                    var x = PlayerStartX + random.Next(-1, 1);
                    var y = PlayerStartY + random.Next(-1, 1);
                    try
                    {
                        map[x, y] = -1;
                    }
                    catch {
                        continue;
                    }
                    AIFirstMove = false;
                    playerPlays = true;
                    continue;
                }
                
                AI(map);
                if (CheckWin(map, out var player))
                {
                    SomeoneWins = true;
                    continue;
                }
                if(!SomethingToPlay(map))
                {
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
        for (int y = 0; y < MapSize; y++) for (int x = 0; x < MapSize; x++) map[x, y] = 0;
        return map;
    }

    static void DrawMap(int[,] map)
    {
        for (int i = 0; i < MapSize; i++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                if (map[y, i] != 0)
                {
                    var customize = map[y, i] == 1 ? "X" : "O";
                    Console.Write($"{customize}    ");
                    continue;
                }
                Console.Write($"{y}{i}   ");
            }
            Console.WriteLine();
        }
    }    

    static bool CheckWin(int[,] map, out int player)
    {
        player = 0;

        for (int p = 0; p < 2; p++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                for (int x = 0; x < MapSize; x++)
                {
                    // check for horizontal
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
                    // check for vertical
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


    static void AI(int[,] map)
    {
        // AI is maximalizer

        var optimalPos = PositionsToCheck(map);

        int maxValue = int.MinValue;
        int bestX = 0;
        int bestY = 0;
        int beta = int.MaxValue;
        int alpha = int.MinValue;
        foreach (var arr in optimalPos)
        {
            map[arr[0], arr[1]] = -1;
            var value = Minimax(map, 7, false, alpha, beta);
            map[arr[0], arr[1]] = 0;
            if (value > maxValue)
            {
                maxValue = value;
                bestX = arr[0];
                bestY = arr[1];
            }
        }
        map[bestX, bestY] = -1;
    }
    
    static int Minimax(int[,] map, int depth, bool isMaximalizer, int alpha, int beta)
    {

        if(CheckWin(map, out int player))
        {
            int outValue = player == 1 ? -10 : 10;
            return outValue * depth;
        }

        if(depth == 0)
        {
            var outValue = CalculateCurrentPosition(map, isMaximalizer);
            return outValue * depth;
        }
        if (!SomethingToPlay(map)) return 0;

        var optimalPos = PositionsToCheck(map);
        // for maximalizer
        if (isMaximalizer)
        {
            int maxValue = int.MinValue;
            int possibleMaxValue = (depth -1) * 10;
            // todo remove full foreach, only foreach optimalPos


            foreach (var arr in optimalPos)
            {
                map[arr[0], arr[1]] = -1;
                var value = Minimax(map, depth - 1, false, alpha, beta);
                map[arr[0], arr[1]] = 0;
                maxValue = Math.Max(value, maxValue);
                alpha = Math.Max(alpha, maxValue);
                if (possibleMaxValue == maxValue) break;
                // beta cut
                if (maxValue >= beta) break;
            }
       
            return maxValue;
        }

        // for minimalizer - player
        if (!isMaximalizer)
        {
            int maxValue = int.MaxValue;
            int possibleMaxValue = (depth - 1) * -10;

            foreach (var arr in optimalPos)
            {
                map[arr[0], arr[1]] = 1;
                var value = Minimax(map, depth - 1, true, alpha, beta);
                map[arr[0], arr[1]] =0 ;
                maxValue = Math.Min(value, maxValue);
                beta = Math.Min(maxValue, beta);
                if (possibleMaxValue == maxValue) break;
                // alpha cut
                if (maxValue <= alpha) break;
            }
            return maxValue;
        }
        return 0;
    }


    static bool SomethingToPlay(int[,] map)
    {
        for (int y = 0; y < MapSize; y++) for (int x = 0; x < MapSize; x++)if(map[x, y] == 0) return true;
        return false;
    }



    static HashSet<int[]> PositionsToCheck(int[,] map)
    {
        HashSet<int[]> optimalPositions = new HashSet<int[]>();
        // Vybírat tahy přímo připojené
		
		for(int y =0;y < MapSize;y++)
		{
			for(int x = 0; x < MapSize; x++)
            {
                if (map[x, y] != 0)
                {
                    // X
                    if(!optimalPositions.Any(arr => arr[0] == x+1 && arr[1] == y)) 
                        if (x + 1 < MapSize && map[x+1, y] == 0) 
                            optimalPositions.Add(new int[] { x + 1, y });

                    if(!optimalPositions.Any(arr => arr[0] == x - 1 && arr[1] == y )) 
                        if (x - 1 < MapSize && x -1 >=0 && map[x-1,y] ==0 )
                            optimalPositions.Add(new int[] { x - 1, y });

                    // Y
                    if(!optimalPositions.Any(arr => arr[0] == x && arr[1]== y + 1 )) 
                        if (y + 1 < MapSize && map[x,y+1] == 0)
                            optimalPositions.Add(new int[] { x , y+1 });

                    if (!optimalPositions.Any(arr => arr[0] == x && arr[1] == y - 1 ))
                        if (y - 1 < MapSize && y - 1 >= 0 && map[x,y-1] == 0) 
                            optimalPositions.Add(new int[] { x , y - 1});

                    //DIAGONAL
                    if (!optimalPositions.Any(arr => arr[0] == x - 1 && arr[1] == y - 1 ))
                        if (x - 1 >= 0 && y - 1 >= 0 && map[x - 1,y-1] == 0)
                            optimalPositions.Add(new int[] { x - 1, y-1 });


                    if (!optimalPositions.Any(arr => arr[0] == x + 1 && arr[1] == y + 1 ))
                        if (x + 1 < MapSize && y + 1 < MapSize && map[x+1,y+1] == 0)
                            optimalPositions.Add(new int[] { x + 1, y + 1 });

                    if(!optimalPositions.Any(arr => arr[0] == x - 1 && arr[1] == y + 1 ))  
                        if(x-1 >= 0 && y+1 < MapSize && map[x-1, y+1] ==0)
                            optimalPositions.Add(new int[] { x - 1, y + 1 });


                    if(!optimalPositions.Contains(new int[] { x + 1, y - 1 })) 
                        if(x+1 < MapSize && y -1 >= 0 && map[x+1, y-1] == 0) 
                            optimalPositions.Add(new int[] { x + 1, y - 1 });

                }
            }
		}
		
        return optimalPositions;
    }

    /// <summary>
    /// Podle počtu jedniček, dvojek, trojek, čtyřek
    /// </summary>
    static int CalculateCurrentPosition(int[,] map, bool isMaximalizer)
    {
        // AI = Maximalizer
        var player = isMaximalizer ? -1 : 1;

        int[] cals = new int[6];

        for (int y = 0; y < MapSize; y++)
        { 
            for (int x = 0; x < MapSize; x++)
            {
                // check for horizontal
                if (x + WinCount <= MapSize)
                {
                    int count = 0;
                    for (int i = 0; i < WinCount; i++)
                    {
                        if (map[x + i, y] == player)
                            count++;
                        else
                        {
                            if (count == 1 || count == 0)
                            {
                                count = 0;
                                continue;
                            }
                            cals[count]++;
                            count = 0;
                        }
                    }
                }
                // check for vertical
                if (y + WinCount <= MapSize)
                {
                    int count = 0;
                    for (int i = 0; i < WinCount; i++)
                    {
                        if (map[x, y + i] == player)
                            count++;
                        else
                        {
                            if (count == 1 || count == 0)
                            {
                                count = 0; 
                                continue;
                            }
                            cals[count]++;
                        }
                    }
                    
                }
                // check for diagonal win
                if (x + WinCount < MapSize + 1 && y + WinCount - 1 < MapSize)
                {
                    int count = 0;
                    for (int i = 0; i < WinCount; i++)
                    {
                        if (map[x + i, y + i] == player) count++;
                        else
                        {
                            if (count == 1 || count == 0)
                            {
                                count = 0;
                                continue;
                            }
                            cals[count]++;
                            count = 0;
                        }
                        
                    }
                }
                // check for anti diagonal
                if (x + WinCount < MapSize + 1 && y - WinCount >= -1)
                {
                    int count = 0;
                    for (int i = 0; i < WinCount; i++)
                    {
                        if (map[x + i, y - i] == player) count++;
                        else
                        {
                            if (count == 1 || count ==0)
                            {
                                count = 0;
                                continue;
                            }
                            cals[count]++;
                            count = 0;
                        }
                    }
                }
            }
        }

        var finalValue = 0;
        for (int i = 0; i < cals.Length; i++)
        {
            finalValue+= cals[i] * i;
        }
        finalValue = isMaximalizer ? (finalValue / 10) : -(finalValue / 10);
        return finalValue;
    }
}
