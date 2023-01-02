using System.Diagnostics;
using System.Text;

namespace TicTacToeAI;
public class Program
{
    public static int MapSize = 10;
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

                AI(map);
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

    static void DrawMap(int[,] map)
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

    static bool CheckWin(int[,] map, out int player)
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


    static double AI(int[,] map)
    {
        // AI is maximalizer
        var optimalPos = PositionsToCheck(map);

        double maxValue = int.MinValue;
        int bestX = 0;
        int bestY = 0;
        double beta = int.MaxValue;
        double alpha = int.MinValue;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        List<EvalPoint> evalPoints = new();
        foreach (var arr in optimalPos)
        {
            map[arr[0], arr[1]] = -1;
            var eval = CalculateCurrentPosition(map, true);
            map[arr[0], arr[1]] = 0;
            evalPoints.Add(new EvalPoint()
            {
                Eval = eval,
                X = arr[1],
                Y = arr[0]
            });
        }
        var orderedPos = evalPoints.OrderByDescending(x => x.Eval).ToList();
        // Kontrola výhry
        foreach (var arr in optimalPos)
        {
            map[arr[0], arr[1]] = -1;
            var value = Minimax(map, 1, false, alpha, beta);
            map[arr[0], arr[1]] = 0;
            if (value > maxValue)
            {
                maxValue = value;
                bestX = arr[0];
                bestY = arr[1];
                if (maxValue >= 10)
                {
                    map[bestX, bestY] = -1;
                    return maxValue;
                }
            }
        }
        maxValue = int.MinValue;
        foreach (var arr in orderedPos)
        {
            map[arr.Y, arr.X] = -1;
            var value = Minimax(map, Depth, false, alpha, beta);
            map[arr.Y, arr.X] = 0;
            if (value > maxValue)
            {
                maxValue = value;
                bestX = arr.Y;
                bestY = arr.X;
                if (maxValue >= 10) break;
            }
        }

        stopwatch.Stop();
        // NO - 4,732 s
        Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"{stopwatch.ElapsedMilliseconds/(double)1000} s");
        map[bestX, bestY] = -1;
        TranspositionTable.ClearTable();
        return maxValue;
    }
    
    static double Minimax(int[,] map, int depth, bool isMaximalizer, double alpha, double beta)
    {
        if(TranspositionTable.DoesPosExists(map, depth, isMaximalizer,out double? storedValue)) return storedValue.Value;

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
        if (!SomethingToPlay(map)) return 0;
        var optimalPos = PositionsToCheck(map);
        List<EvalPoint> evalPoints = new();
        foreach (var arr in optimalPos)
        {
            var isMax = isMaximalizer ? -1 : 1;
            map[arr[0], arr[1]] = isMax;
            var eval = CalculateCurrentPosition(map, isMaximalizer);
            map[arr[0], arr[1]] = 0;
            evalPoints.Add(new EvalPoint()
            {
                Eval = eval,
                X = arr[1],
                Y = arr[0]
            });
        }

        var avg =  Math.Round(evalPoints.Average(x => x.Eval),1);

        // for maximalizer
        if (isMaximalizer)
        {
            double maxValue = int.MinValue;
            int possibleMaxValue = (depth -1) * 10;
            var p = evalPoints.OrderByDescending(x => x.Eval);
            foreach (var arr in p) 
            {
                map[arr.Y, arr.X] = -1;
                double value = Minimax(map, depth - 1, false, alpha, beta);
                map[arr.Y, arr.X] = 0;
                maxValue = Math.Max(value, maxValue);
                alpha = Math.Max(alpha, maxValue);
                if (maxValue >= beta) break;
                if (possibleMaxValue == maxValue) break;
            }
       
            return maxValue;
        }
        // for minimalizer - player
        if (!isMaximalizer)
        {
            double maxValue = int.MaxValue;
            int possibleMaxValue = (depth - 1) * -10;
            var p = evalPoints.OrderBy(x => x.Eval);
            foreach (var arr in p)
            {
                map[arr.Y, arr.X] = 1;
                double value = Minimax(map, depth - 1, true, alpha, beta);
                map[arr.Y, arr.X] = 0;
                maxValue = Math.Min(value, maxValue);
                beta = Math.Min(maxValue, beta);
                if (maxValue <= alpha) break;
                if (possibleMaxValue == maxValue) break;                
            }
            return maxValue;
        }
        return 0;
    }

    static bool SomethingToPlay(int[,] map)
    {
        for (int y = 0; y < MapSize; y++) for (int x = 0; x < MapSize; x++) if(map[x, y] == 0) return true;
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
                if (map[y, x] != 0)
                {
                    // X
                    //+1
                    if(!optimalPositions.Any(arr => arr[0] == y+1 && arr[1] == x)) 
                        if (y + 1 < MapSize && map[y+1, x] == 0) 
                            optimalPositions.Add(new int[] { y + 1, x});

                    //+2
                    /*
                    if (!optimalPositions.Any(arr => arr[0] == x + 2 && arr[1] == y))
                        if (x + 2 < MapSize && map[x + 2, y] == 0)
                            optimalPositions.Add(new int[] { x + 2, y });
                    
                    */
                    // +1
                    if (!optimalPositions.Any(arr => arr[0] == y - 1 && arr[1] == x )) 
                        if (y - 1 < MapSize && y -1 >=0 && map[y-1,x] ==0 )
                            optimalPositions.Add(new int[] { y - 1, x });
                    /*
                    // +2
                    
                    if (!optimalPositions.Any(arr => arr[0] == x - 2 && arr[1] == y))
                        if (x - 2 < MapSize && x - 2 >= 0 && map[x - 2, y] == 0)
                            optimalPositions.Add(new int[] { x - 2, y });
                    */
                    // Y
                    // +1
                    if (!optimalPositions.Any(arr => arr[0] == y && arr[1]== x + 1 )) 
                        if (x + 1 < MapSize && map[y,x+1] == 0)
                            optimalPositions.Add(new int[] { y , x+1 });

                    /*
                    if (!optimalPositions.Any(arr => arr[0] == x && arr[1] == y + 2))
                        if (y + 2 < MapSize && map[x, y + 2] == 0)
                            optimalPositions.Add(new int[] { x, y + 2 });

                    */
                    //+1
                    if (!optimalPositions.Any(arr => arr[0] == y && arr[1] == x - 1))
                        if (x - 1 < MapSize && x - 1 >= 0 && map[y, x -1] == 0) 
                            optimalPositions.Add(new int[] { y , x - 1});

                    /*
                    // +2
                    if (!optimalPositions.Any(arr => arr[0] == x && arr[1] == y - 2))
                        if (y - 2 < MapSize && y - 2 >= 0 && map[x, y - 2] == 0)
                            optimalPositions.Add(new int[] { x, y - 2 });
                    */

                    //DIAGONAL
                    if (!optimalPositions.Any(arr => arr[0] == y - 1 && arr[1] == x - 1 ))
                        if (x - 1 >= 0 && y - 1 >= 0 && map[y - 1,x-1] == 0)
                            optimalPositions.Add(new int[] { y - 1, x-1 });

                    /*
                    if (!optimalPositions.Any(arr => arr[0] == x - 2 && arr[1] == y - 2))
                        if (x - 2 >= 0 && y - 2 >= 0 && map[x - 2, y - 2] == 0)
                            optimalPositions.Add(new int[] { x - 2, y - 2 });

                    */

                    // +1
                    if (!optimalPositions.Any(arr => arr[0] == y + 1 && arr[1] == x + 1 ))
                        if (x + 1 < MapSize && y + 1 < MapSize && map[y+1,x+1] == 0)
                            optimalPositions.Add(new int[] { y + 1, x + 1 });

                    /*
                    // +2
                    if (!optimalPositions.Any(arr => arr[0] == x + 2 && arr[1] == y + 2))
                        if (x + 2 < MapSize && y + 2 < MapSize && map[x + 2, y + 2] == 0)
                            optimalPositions.Add(new int[] { x + 2, y + 2 });
                    */

                    //+1
                    if (!optimalPositions.Any(arr => arr[0] == y - 1 && arr[1] == x + 1 ))  
                        if(y-1 >= 0 && x+1 < MapSize && map[y-1, x+1] ==0)
                            optimalPositions.Add(new int[] { y - 1, x + 1 });
                    /*
                    //+2
                    if (!optimalPositions.Any(arr => arr[0] == x - 2 && arr[1] == y + 2))
                        if (x - 2 >= 0 && y + 2 < MapSize && map[x - 2, y + 2] == 0)
                            optimalPositions.Add(new int[] { x - 2, y + 2 });
                    */
                    // +1
                    if (!optimalPositions.Any(arr => arr[0] ==  y + 1 && arr[1] == x - 1 )) 
                        if(y+1 < MapSize && x -1 >= 0 && map[y+1, x-1] == 0) 
                            optimalPositions.Add(new int[] { y + 1, x - 1 });
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

    public static double CalculateCurrentPosition(int[,] map, bool isMaximalizer)
    {
        // AI = Maximalizer
        var player = isMaximalizer ? -1 : 1;
        List<List<Point>> positionStates = new();
        for (int y = 0; y < MapSize; y++) 
            for (int x = 0; x < MapSize; x++)
                if (map[y,x] ==  player)
                    BFS(x, y, map, positionStates, player);

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
        var reversedPlayer = isMaximalizer ? 1 : -1;
        bool[,] globalVisited = new bool[MapSize, MapSize];

        foreach (var state in sortedBySize) finalValue += state.Count / (double)10;
        
        finalValue = isMaximalizer ? (finalValue) : -(finalValue);
        return  Math.Round(finalValue,2);
    }

    static void BFS(int startX, int startY, int[,] map, List<List<Point>> positionStates, int player)
    {
        Queue<Point> queue = new();
        Point start = new();
        start.X = startX;
        start.Y = startY;
        bool[,] visited = new bool[MapSize, MapSize];
        var neighbours = GetNeighbours(start, visited, map, player);


        List<Point> outPoints = new();
        if (neighbours.Count() == 0)
        {
            outPoints.Add(start);
            positionStates.Add(outPoints);
        }

        foreach (var neighbour in neighbours) queue.Enqueue(neighbour);

        while(queue.Count > 0)
        {
            var currPoint = queue.Dequeue();
            visited[currPoint.Y, currPoint.X] = true;

            // Nalezení dalšího bodu v cestě
            if (currPoint.X + currPoint.DiffX >= 0 && currPoint.X + currPoint.DiffX < MapSize && currPoint.Y + currPoint.DiffY >= 0 && currPoint.Y + currPoint.Y < MapSize)
            {
                if (map[currPoint.Y + currPoint.DiffY, currPoint.X + currPoint.DiffX] == player)
                {
                    queue.Enqueue(new Point()
                    {
                        DiffX = currPoint.DiffX,
                        DiffY = currPoint.DiffY,
                        Parent = currPoint,
                        X = currPoint.X + currPoint.DiffX,
                        Y = currPoint.Y + currPoint.DiffY
                    });
                    continue;
                }
            }
            // Pokud zde není uložíme cestu
            var parent = currPoint;
            outPoints = new();
            if (parent.Parent == null)
            {
                positionStates.Add(outPoints);
                continue;
            }
            while (parent != null)
            {
                outPoints.Add(parent);
                parent = parent.Parent;
            }

            var count = outPoints.Count;
            var maxSameCount = 0;
            bool toBreak = false;
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
            if (toBreak) continue;
            positionStates.Add(outPoints);
        }
    }

    static IEnumerable<Point> GetNeighbours(Point currentPoint, bool[,] visited, int[,] map, int player)
    {

        List<Point> allPoints = new List<Point>()
        {
            // X
            new Point(){ X = currentPoint.X + 1, Y = currentPoint.Y, Parent = currentPoint, DiffX = 1},
            new Point(){ X = currentPoint.X - 1, Y = currentPoint.Y, Parent = currentPoint, DiffX = -1, },

            //Y
            new Point(){ X = currentPoint.X, Y = currentPoint.Y + 1, Parent = currentPoint, DiffY = 1},
            new Point(){ X = currentPoint.X, Y = currentPoint.Y - 1, Parent = currentPoint,  DiffY = -1},

            // Dia
            new Point(){ X = currentPoint.X - 1, Y = currentPoint.Y - 1, Parent = currentPoint, DiffX = -1, DiffY = -1},
            new Point(){ X = currentPoint.X + 1, Y = currentPoint.Y + 1, Parent = currentPoint, DiffX = 1, DiffY = 1},

            // Dia
            new Point(){ X = currentPoint.X - 1, Y = currentPoint.Y + 1, Parent = currentPoint, DiffX = -1, DiffY = 1},
            new Point(){ X = currentPoint.X + 1, Y = currentPoint.Y - 1, Parent = currentPoint, DiffX = 1, DiffY = -1},
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

        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {1,0,-1,0,0},
            {1,-1,-1,-1,1},
            {0,0,-1,0,0},
            {0,0,0,0,0}
        };
        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 1.4);

        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {1,0,0,-1,0},
            {1,-1,-1,-1,0},
            {0,0,-1,0,0},
            {0,0,0,0,0}
        };
        eval = CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 1.3);
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

    public static void AITests()
    {
        var testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {1,0,0,0,0},
            {-1,-1,-1,0,0},
            {0,0,-1,0,0},
            {0,0,-1,0,0}
        };
        MapSize = 5;
        var eval = AI(testMap);
        DrawMap(testMap);
        AreEqual(eval, 10);


        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {1,0,0,0,0},
            {0,0,-1,0,0},
            {0,-1,-1,-1,0},
            {0,0,-1,0,0}
        };
        Depth = 5;
        eval = AI(testMap);
        DrawMap(testMap);
        AreEqual(eval, 10);


        testMap = new int[7, 7]
        {
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,-1,-1,0,0,0},
            {0,0,-1,0,-1,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0}
        };
        Depth = 5;
        MapSize = 7;
        eval = AI(testMap);
        DrawMap(testMap);
        AreEqual(eval, 30);

        testMap = new int[7, 7]
        {
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,1,-1,-1,-1,1},
            {0,0,-1,-1,0,0,0},
            {0,0,-1,0,0,0,0},
            {0,0,-1,0,0,0,0},
            {0,0,1,0,0,0,0}
        };
        Depth = 5;
        WinCount = 5;
        MapSize = 7;
        eval = AI(testMap);
        DrawMap(testMap);
        AreEqual(eval, 2.8);

    }
}

public class Point : PointBase
{
    public Point Parent { get; set; }
    public int DiffX { get; set; } = 0;
    public int DiffY { get; set; } = 0;
}

public class PointBase
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class EvalPoint : PointBase
{
    public double Eval { get; set; }
}
static public class TranspositionTable
{
    static Dictionary<string, double> transpositionTable = new Dictionary<string, double>();
    static public bool DoesPosExists(int[,] map,int depth ,bool isMaximalizer, out double? storedValue)
    {
        storedValue = null;
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < Program.MapSize; y++) for (int x = 0; x < Program.MapSize; x++) sb.Append(map[y, x]);
        var positionString = $"{sb.ToString()}|{isMaximalizer}|{depth}";
        if (!transpositionTable.TryGetValue(positionString, out var value))
        {
            var eval = Program.CalculateCurrentPosition(map, isMaximalizer);
            transpositionTable.Add(positionString, eval);
            return false;
        }
        storedValue = value;
        return true;
    }
    public static void ClearTable() => transpositionTable.Clear();
}