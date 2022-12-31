namespace TicTacToeAI;
public class Program
{
    static int MapSize = 10;
    static int WinCount = 5;
    static int Depth = 5;
    static bool SomeoneWins = false;
    static int[] players = { 1, -1 };
    static bool AIFirstMove = true;
    static bool PlayerStarted { get; set; }
    static int PlayerStartX { get; set; }
    static int PlayerStartY { get; set; } 

    // TODO 2)Multithreading +  1)TransitionTables => fast depth 5 and more
    public static void Main(string[] args)
    {

        EvalutationTests();
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
        do
        {
            if (playerPlays)
            {
                DrawMap(map);
                var posX = int.Parse(Console.ReadLine());
                var posY = int.Parse(Console.ReadLine());
                if (map[posX, posY] != 0) continue;
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
                    /*
                    var x = PlayerStartX + random.Next(-1, 1);
                    var y = PlayerStartY + random.Next(-1, 1);
                    */
                    var x = 5;
                    var y = 5;

                    try
                    {
                        map[x, y] = -1;
                    }
                    catch
                    {
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
                if (!SomethingToPlay(map))
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

        double maxValue = int.MinValue;
        int bestX = 0;
        int bestY = 0;
        double beta = int.MaxValue;
        double alpha = int.MinValue;
        foreach (var arr in optimalPos)
        {
            map[arr[0], arr[1]] = -1;
            var value = Minimax(map, Depth, false, alpha, beta);
            map[arr[0], arr[1]] = 0;
            if (value > maxValue)
            {
                maxValue = value;
                bestX = arr[0];
                bestY = arr[1];
                if (maxValue >= 10)
                    break;
            }
        }
        map[bestX, bestY] = -1;
    }
    
    static double Minimax(int[,] map, int depth, bool isMaximalizer, double alpha, double beta)
    {

        if(CheckWin(map, out int player))
        {
            int outValue = player == 1 ? -10 : 10;
            return outValue * depth;
        }

        if(depth == 0)
        {
            var outValue = CalculateCurrentPosition(map, isMaximalizer);
            return outValue;
        }
        if (!SomethingToPlay(map)) 
            return 0;

        var optimalPos = PositionsToCheck(map);
        // for maximalizer
        if (isMaximalizer)
        {
            double maxValue = int.MinValue;
            int possibleMaxValue = (depth -1) * 10;
            foreach (var arr in optimalPos)
            {
                map[arr[0], arr[1]] = -1;
                double value = Minimax(map, depth - 1, false, alpha, beta);
                map[arr[0], arr[1]] = 0;
                maxValue = Math.Max(value, maxValue);
                if (maxValue >= beta) break;
                alpha = Math.Max(alpha, maxValue);
                if (possibleMaxValue == maxValue) break;
            }
       
            return maxValue;
        }

        // for minimalizer - player
        if (!isMaximalizer)
        {
            double maxValue = int.MaxValue;
            int possibleMaxValue = (depth - 1) * -10;

            foreach (var arr in optimalPos)
            {
                map[arr[0], arr[1]] = 1;
                double value = Minimax(map, depth - 1, true, alpha, beta);
                map[arr[0], arr[1]] = 0;
                maxValue = Math.Min(value, maxValue);
                if (maxValue <= alpha) break;
                beta = Math.Min(maxValue, beta);
                if (possibleMaxValue == maxValue) break;                
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
                    //+1
                    if(!optimalPositions.Any(arr => arr[0] == x+1 && arr[1] == y)) 
                        if (x + 1 < MapSize && map[x+1, y] == 0) 
                            optimalPositions.Add(new int[] { x + 1, y });

                    //+2
                    /*
                    if (!optimalPositions.Any(arr => arr[0] == x + 2 && arr[1] == y))
                        if (x + 2 < MapSize && map[x + 2, y] == 0)
                            optimalPositions.Add(new int[] { x + 2, y });
                    */

                    // +1
                    if (!optimalPositions.Any(arr => arr[0] == x - 1 && arr[1] == y )) 
                        if (x - 1 < MapSize && x -1 >=0 && map[x-1,y] ==0 )
                            optimalPositions.Add(new int[] { x - 1, y });

                    // +2
                    /*
                    if (!optimalPositions.Any(arr => arr[0] == x - 2 && arr[1] == y))
                        if (x - 2 < MapSize && x - 2 >= 0 && map[x - 2, y] == 0)
                            optimalPositions.Add(new int[] { x - 2, y });
                    */
                    // Y
                    // +1
                    if (!optimalPositions.Any(arr => arr[0] == x && arr[1]== y + 1 )) 
                        if (y + 1 < MapSize && map[x,y+1] == 0)
                            optimalPositions.Add(new int[] { x , y+1 });

                    /*
                    if (!optimalPositions.Any(arr => arr[0] == x && arr[1] == y + 2))
                        if (y + 2 < MapSize && map[x, y + 2] == 0)
                            optimalPositions.Add(new int[] { x, y + 2 });
                    */

                    //+1
                    if (!optimalPositions.Any(arr => arr[0] == x && arr[1] == y - 1 ))
                        if (y - 1 < MapSize && y - 1 >= 0 && map[x,y-1] == 0) 
                            optimalPositions.Add(new int[] { x , y - 1});

                    /*
                    // +2
                    if (!optimalPositions.Any(arr => arr[0] == x && arr[1] == y - 2))
                        if (y - 2 < MapSize && y - 2 >= 0 && map[x, y - 2] == 0)
                            optimalPositions.Add(new int[] { x, y - 2 });
                    */

                    //DIAGONAL
                    if (!optimalPositions.Any(arr => arr[0] == x - 1 && arr[1] == y - 1 ))
                        if (x - 1 >= 0 && y - 1 >= 0 && map[x - 1,y-1] == 0)
                            optimalPositions.Add(new int[] { x - 1, y-1 });

                    /*
                    if (!optimalPositions.Any(arr => arr[0] == x - 2 && arr[1] == y - 2))
                        if (x - 2 >= 0 && y - 2 >= 0 && map[x - 2, y - 2] == 0)
                            optimalPositions.Add(new int[] { x - 2, y - 2 });
                    */


                    // +1
                    if (!optimalPositions.Any(arr => arr[0] == x + 1 && arr[1] == y + 1 ))
                        if (x + 1 < MapSize && y + 1 < MapSize && map[x+1,y+1] == 0)
                            optimalPositions.Add(new int[] { x + 1, y + 1 });

                    /*
                    // +2
                    if (!optimalPositions.Any(arr => arr[0] == x + 2 && arr[1] == y + 2))
                        if (x + 2 < MapSize && y + 2 < MapSize && map[x + 2, y + 2] == 0)
                            optimalPositions.Add(new int[] { x + 2, y + 2 });
                    */

                    //+1
                    if (!optimalPositions.Any(arr => arr[0] == x - 1 && arr[1] == y + 1 ))  
                        if(x-1 >= 0 && y+1 < MapSize && map[x-1, y+1] ==0)
                            optimalPositions.Add(new int[] { x - 1, y + 1 });
                    /*
                    //+2
                    if (!optimalPositions.Any(arr => arr[0] == x - 2 && arr[1] == y + 2))
                        if (x - 2 >= 0 && y + 2 < MapSize && map[x - 2, y + 2] == 0)
                            optimalPositions.Add(new int[] { x - 2, y + 2 });
                    */
                    // +1
                    if (!optimalPositions.Any(arr => arr[0] ==  x + 1 && arr[1] == y - 1 )) 
                        if(x+1 < MapSize && y -1 >= 0 && map[x+1, y-1] == 0) 
                            optimalPositions.Add(new int[] { x + 1, y - 1 });
                    // +2
                    /*
                    if (!optimalPositions.Any(arr => arr[0] == x + 2 && arr[1] == y - 2))
                        if (x + 2 < MapSize && y - 2 >= 0 && map[x + 2, y - 2] == 0)
                            optimalPositions.Add(new int[] { x + 2, y - 2 });

                    */
                }
            }
		}
		
        return optimalPositions;
    }

    static double CalculateCurrentPosition(int[,] map, bool isMaximalizer)
    {
        // AI = Maximalizer
        var player = isMaximalizer ? -1 : 1;
        List<List<DFSPoint>> positionStates = new();
        for (int y = 0; y < MapSize; y++) 
            for (int x = 0; x < MapSize; x++)
                if (map[y,x] ==  player)
                    DFS(x, y, map, positionStates, player);

        var sortedBySize = positionStates.OrderByDescending(x => x.Count).ToList();

        for (int i = 0; i < sortedBySize.Count; i++)
        {
            foreach (var itemToCheck in sortedBySize.ToList())
            {
                if (itemToCheck == sortedBySize[i]) continue;

                bool allSame = true;
                foreach (var currChecked in itemToCheck)
                {
                    if(!sortedBySize[i].Any(x => x.X == currChecked.X && x.Y == currChecked.Y))
                    {
                        allSame = false;
                        break;
                    }
                }
                if (allSame) sortedBySize.Remove(itemToCheck);
            }
        }

        double finalValue = 0;
        foreach (var state in sortedBySize) finalValue += state.Count / (double)10;
        finalValue = isMaximalizer ? (finalValue) : -(finalValue);
        return  Math.Round(finalValue,2);
    }

    static void DFS(int startX, int startY, int[,] map, List<List<DFSPoint>> positionStates, int player)
    {
        Stack<DFSPoint> stack = new Stack<DFSPoint>();
        DFSPoint start = new();
        start.X = startX;
        start.Y = startY;
        if(startX == 3 && startY == 2)
        {

        }

        bool[,] notNeight = new bool[MapSize, MapSize];
        bool[,] globalVisited = new bool[MapSize, MapSize];
        stack.Push(start);
        int count = 0;
        int maxSameCount = 0;
        while (stack.Any())
        {
            var currentP = stack.Pop();
            globalVisited[currentP.Y, currentP.X] = true;
            var neighbours = GetNeighbours(currentP, globalVisited, map, player);

            if (!neighbours.Any()) 
            {
                if (notNeight[currentP.Y, currentP.X]) continue;
                notNeight[currentP.Y, currentP.X] = true;
                List<DFSPoint> outPoints = new();
                var parent = currentP;
                outPoints.Add(parent);
                int xDiff = 0;
                int yDiff = 0;

                bool first = true;
                if (parent.Parent == null)
                {
                    positionStates.Add(outPoints);
                    continue;
                }
                bool toBreak = false;
                while (parent != null)
                {
                    currentP = parent;
                    parent = parent.Parent;


                    if (first)
                    {
                        xDiff = parent.X - currentP.X;
                        yDiff = parent.Y - currentP.Y;
                    }
                    
                    if(parent != null && (xDiff != parent.X - currentP.X || yDiff != parent.Y - currentP.Y) && !first)
                    {
                        // kontrola jiného směru
                        xDiff = parent.X - currentP.X;
                        yDiff = parent.Y - currentP.Y;

                        count = outPoints.Count;
                        maxSameCount = 0;
                        foreach (var ps in positionStates)
                        {
                            int checkCount = 0;
                            foreach (var p in outPoints)
                            {
                                if (ps.Any(x => x.X == p.X && x.Y == p.Y)) checkCount++;
                            }
                            if (checkCount > maxSameCount) maxSameCount = checkCount;
                            if (count == maxSameCount) 
                            {
                                toBreak = true;
                                break;
                            }
                        }
                        if (!toBreak)
                        {
                            positionStates.Add(outPoints);
                            outPoints = new();
                            outPoints.Add(currentP);
                        }
                    }
                    if(parent != null) outPoints.Add(parent);
                    first = false;
                }
                if (toBreak) continue;
                count = outPoints.Count;
                maxSameCount = 0;
                foreach (var ps in positionStates)
                {
                    int checkCount = 0;
                    foreach (var p in outPoints)
                    {
                        if (ps.Any(x => x.X == p.X && x.Y == p.Y)) checkCount++;
                    }
                    if (checkCount > maxSameCount) maxSameCount = checkCount;
                    if (count == maxSameCount)
                    {
                        toBreak = true;
                        break;
                    }
                }
                if(!toBreak) positionStates.Add(outPoints);
            }
            foreach (var n in neighbours) stack.Push(n);
        }
    }

    static IEnumerable<DFSPoint> GetNeighbours(DFSPoint currentPoint, bool[,] visited, int[,] map, int player)
    {

        List<DFSPoint> allPoints = new List<DFSPoint>()
        {
            // X
            new DFSPoint(){ X = currentPoint.X + 1, Y = currentPoint.Y, Parent = currentPoint},
            new DFSPoint(){ X = currentPoint.X - 1, Y = currentPoint.Y, Parent = currentPoint},

            //Y
            new DFSPoint(){ X = currentPoint.X, Y = currentPoint.Y + 1, Parent = currentPoint},
            new DFSPoint(){ X = currentPoint.X, Y = currentPoint.Y - 1, Parent = currentPoint},

            // Dia
            new DFSPoint(){ X = currentPoint.X - 1, Y = currentPoint.Y - 1, Parent = currentPoint},
            new DFSPoint(){ X = currentPoint.X + 1, Y = currentPoint.Y + 1, Parent = currentPoint},

            // Dia
            new DFSPoint(){ X = currentPoint.X - 1, Y = currentPoint.Y + 1, Parent = currentPoint},
            new DFSPoint(){ X = currentPoint.X + 1, Y = currentPoint.Y - 1, Parent = currentPoint},
        };
        var possiblePoints = allPoints.Where(p => p.X >= 0 && p.X < MapSize && p.Y >= 0 && p.Y < MapSize && !visited[p.Y, p.X] && map[p.Y, p.X] == player).ToList();
        
        return possiblePoints;
    }

    static void EvalutationTests() 
    {
        int[,] testMap = new int[5,5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };

        MapSize = 5;
        var eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0);

        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,0,-1,0,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };

        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.1);

        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,-1,-1,0,0 },
            { 0,0,-1,0,0 },
            { 0,0,0,0,0 }
        };
        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.6);

        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,-1,-1,-1,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };
        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.3);


        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,-1,-1,-1,-1 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };
        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.4);

        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,-1,-1,-1,0 },
            { 0,0,-1,0,0 },
            { 0,0,0,0,0 }
        };
        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.9);


        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { -1,-1,-1,-1,-1 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };
        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.5);



        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 1,0,0,0,0 },
            { 1,-1,-1,-1,1 },
            { -1,0,0,0,0 },
            { 0,0,0,0,0 }
        };
        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.5);

        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {-1,0,0,0,0},
            {-1,1,1,1,-1},
            {0,0,1,0,0},
            {0,0,0,0,0}
        };
        eval = CalculateCurrentPosition(testMap, false);
        AreEqual(eval, -0.9);


        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {-1,0,1,0,0},
            {-1,1,1,1,-1},
            {0,0,1,0,0},
            {0,0,0,0,0}
        };
        eval = CalculateCurrentPosition(testMap, false);
        AreEqual(eval, -1.4);
    }

    static void AreEqual(double eval, double exepted)
    {
        var equal = eval == exepted ? ConsoleColor.Green : ConsoleColor.Red;
        
        Console.ForegroundColor = equal;
        if (eval == exepted)
        {
            Console.WriteLine("Passed");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Error");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"calculated eval: {eval}");
            Console.WriteLine($"exepted eval: {exepted}");
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
}

public class DFSPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public DFSPoint Parent { get; set; }
}