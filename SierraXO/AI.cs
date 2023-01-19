using System.Diagnostics;
using System.Text;

namespace TicTacToeAI;
public class AI
{
    public static int Depth = 5;
    public static int positionEvaluated = 0;

    static Stopwatch evalTime = new();
    public static double GetAIMove(int[,] map , out int X, out int Y)
    {
        // AI is maximalizer
        var optimalPos = PositionsToCheck(map);
        int bestX = 0;
        int bestY = 0;
        double beta = int.MaxValue;
        double alpha = int.MinValue;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        List<EvalPoint> evalPoints = new();

        bool defendNeeded = false;
        bool outDef;

        foreach (var arr in optimalPos)
        {
            map[arr[0], arr[1]] = -1;
            var eval = CalculateCurrentPosition(map, true, out outDef);
            if (outDef) defendNeeded = true;
            map[arr[0], arr[1]] = 0;
            evalPoints.Add(new EvalPoint()
            {
                Eval = eval,
                X = arr[1],
                Y = arr[0]
            });
        }

        double maxValue = int.MinValue;

        var maxEval = evalPoints.Max(x => x.Eval);

        var points = defendNeeded ? evalPoints.Where(x => x.Eval == maxEval) : evalPoints.OrderByDescending(x => x.Eval);
        //var points = evalPoints.OrderByDescending(x => x.Eval);

        foreach (var arr in points)
        {
            map[arr.Y, arr.X] = -1;
            var value = Minimax(map, Depth, false, alpha, beta);
            map[arr.Y, arr.X] = 0;
            if (value > maxValue)
            {
                maxValue = value;
                bestX = arr.Y;
                bestY = arr.X;
                if (value == Depth * 10000) break;
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"Total Runtime {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Total Runtime {stopwatch.ElapsedMilliseconds / (double)1000} s");
        Console.WriteLine($"Moves evaluated {positionEvaluated}");
        Console.WriteLine($"Eval time {evalTime.ElapsedMilliseconds} ms");
        Console.WriteLine($"Eval time {evalTime.ElapsedMilliseconds / (double)1000} s");
        map[bestX, bestY] = -1;
        Console.WriteLine($"Y = {bestX}, X = {bestY}");
        TranspositionTable.ClearTable();
        positionEvaluated = 0;

        X = bestX;
        Y = bestY;
        evalTime.Reset();
        defendNeeded = false;
        return maxValue;
    }

    static double Minimax(int[,] map, int depth, bool isMaximalizer, double alpha, double beta)
    {
        bool outDef;
        positionEvaluated++;
        if (TranspositionTable.DoesPosExists(map, depth, isMaximalizer, out double? storedValue)) return storedValue.Value;

        if (Game.CheckWin(map, out int player))
        {
            int outValue = player == 1 ? -10000 : 10000;
            return outValue * depth;
        }
        if (depth == 0)
        {
            var outValue = CalculateCurrentPosition(map, isMaximalizer, out outDef);
            return outValue;
        }

        if (!Game.SomethingToPlay(map)) return 0;

        var optimalPos = PositionsToCheck(map);
        List<EvalPoint> evalPoints = new();
        bool defendNeeded = false;

        foreach (var arr in optimalPos)
        {
            var isMax = isMaximalizer ? -1 : 1;
            map[arr[0], arr[1]] = isMax;
            var eval = CalculateCurrentPosition(map, isMaximalizer, out outDef);
            if (outDef) defendNeeded = true;
            map[arr[0], arr[1]] = 0;
            evalPoints.Add(new EvalPoint()
            {
                Eval = eval,
                X = arr[1],
                Y = arr[0]
            });
        }

        var avg = Math.Round(evalPoints.Average(x => x.Eval), 2);
        var maxEval = evalPoints.Max(x => x.Eval);
        // for maximalizer
        if (isMaximalizer)
        {
            double maxValue = int.MinValue;
            int possibleMaxValue = depth == 1 ? 10000 : (depth - 2) * 10000;
            var points = defendNeeded ? evalPoints.Where(x => x.Eval == maxEval) : evalPoints.Where(x => x.Eval >= avg).OrderByDescending(x => x.Eval);
            //var p = evalPoints.OrderByDescending(x => x.Eval);
            foreach (var arr in points)
            {
                map[arr.Y, arr.X] = -1;
                double value = Minimax(map, depth - 1, false, alpha, beta);
                map[arr.Y, arr.X] = 0;
                maxValue = Math.Max(value, maxValue);
                alpha = Math.Max(alpha, maxValue);

                if (maxValue > beta) break;
                if (possibleMaxValue == maxValue) break;
            }
            return maxValue;
        }

        // for minimalizer - player
        if (!isMaximalizer)
        {
            double maxValue = int.MaxValue;
            int possibleMaxValue = depth == 1 ? -10000 : (depth - 2) * -10000;
            var points = defendNeeded ? evalPoints.Where(x => x.Eval == maxEval) : evalPoints.Where(x => x.Eval < avg).OrderBy(x => x.Eval >= avg);
            //var p = evalPoints.OrderBy(x => x.Eval);
            foreach (var arr in points)
            {
                // https://www.chessprogramming.org/Alpha-Beta
                map[arr.Y, arr.X] = 1;
                double value = Minimax(map, depth - 1, true, alpha, beta);
                maxValue = Math.Min(maxValue, value);
                map[arr.Y, arr.X] = 0;

                if (value < alpha) break;
                beta = Math.Min(beta, maxValue);
                if (possibleMaxValue == maxValue) break;
            }
            return maxValue;
        }
        return 0;
    }

    static HashSet<int[]> PositionsToCheck(int[,] map)
    {
        HashSet<int[]> optimalPositions = new HashSet<int[]>();

        for (int y = 0; y < Game.MapSize; y++)
        {
            for (int x = 0; x < Game.MapSize; x++)
            {
                if (map[y, x] != 0)
                {
                    for (int i = 1; i < 2; i++)
                    {
                        if (!optimalPositions.Any(arr => arr[0] == y + i && arr[1] == x))
                            if (y + i < Game.MapSize && map[y + i, x] == 0)
                                optimalPositions.Add(new int[] { y + i, x });

                        if (!optimalPositions.Any(arr => arr[0] == y - i && arr[1] == x))
                            if (y - i < Game.MapSize && y - i >= 0 && map[y - i, x] == 0)
                                optimalPositions.Add(new int[] { y - i, x });

                        if (!optimalPositions.Any(arr => arr[0] == y && arr[1] == x + i))
                            if (x + i < Game.MapSize && map[y, x + i] == 0)
                                optimalPositions.Add(new int[] { y, x + i });

                        if (!optimalPositions.Any(arr => arr[0] == y && arr[1] == x - i))
                            if (x - i < Game.MapSize && x - i >= 0 && map[y, x - i] == 0)
                                optimalPositions.Add(new int[] { y, x - i });

                        if (!optimalPositions.Any(arr => arr[0] == y - i && arr[1] == x - i))
                            if (x - i >= 0 && y - i >= 0 && map[y - i, x - i] == 0)
                                optimalPositions.Add(new int[] { y - i, x - i });

                        if (!optimalPositions.Any(arr => arr[0] == y + i && arr[1] == x + i))
                            if (x + i < Game.MapSize && y + i < Game.MapSize && map[y + i, x + i] == 0)
                                optimalPositions.Add(new int[] { y + i, x + i });

                        if (!optimalPositions.Any(arr => arr[0] == y - i && arr[1] == x + i))
                            if (y - i >= 0 && x + i < Game.MapSize && map[y - i, x + i] == 0)
                                optimalPositions.Add(new int[] { y - i, x + i });

                        if (!optimalPositions.Any(arr => arr[0] == y + i && arr[1] == x - i))
                            if (y + i < Game.MapSize && x - i >= 0 && map[y + i, x - i] == 0)
                                optimalPositions.Add(new int[] { y + i, x - i });
                    }
                }
            }
        }
        return optimalPositions;
    }

    public static double CalculateCurrentPosition(int[,] map, bool isMaximalizer, out bool defendNeeded)
    {
        defendNeeded = false;
        evalTime.Start();
        var player = isMaximalizer ? -1 : 1;
        double myAttack = 0;
        double enemyAttack = 0;
        bool mustDefend = false;

        var reversedPlayer = isMaximalizer ? 1 : -1;
        for (int y = 0; y < Game.MapSize; y++)
        {
            for (int x = 0; x < Game.MapSize; x++)
            {
                if (map[y, x] == reversedPlayer)
                {
                    BFS(x, y, map, positionStates, reversedPlayer);
                }
            }
        }
        foreach (var state in positionStates.Where(x => x.Count > 1))
        {
            foreach (var op in state)
            {
                var p = alonePoints.FirstOrDefault(x => x.X == op.X && x.Y == op.Y);
                if (p != null) alonePoints.Remove(p);

            }
        }

        
        enemyAttack += alonePoints.Count * 10;

        foreach (var state in positionStates)
        {
            if (GetValueForCurrentState(state, map, out var attack))
            {
                mustDefend = true;
                defendNeeded = true;
            }
            enemyAttack += attack;
        }
       
        ResetVisited();
        for (int y = 0; y < Game.MapSize; y++)
        {
            for (int x = 0; x < Game.MapSize; x++)
            {
                if (map[y, x] == player)
                {
                    BFS(x, y, map, positionStates, player);
                }
            }
        }

        foreach (var state in positionStates.Where(x => x.Count > 1))
        {
            foreach (var op in state)
            {
                var p = alonePoints.FirstOrDefault(x => x.X == op.X && x.Y == op.Y);
                if (p != null) alonePoints.Remove(p);

            }
        }


        myAttack += alonePoints.Count * 10;
        foreach (var state in positionStates)
        {
            GetValueForCurrentState(state, map, out var attack, mustDefend);
            myAttack += attack;
        }


        enemyAttack = isMaximalizer ? -enemyAttack : enemyAttack;
        myAttack = isMaximalizer ? myAttack : -myAttack;

        var evaluatedValue = myAttack + enemyAttack;
        evalTime.Stop();

        ResetVisited();
        return evaluatedValue;
    }

    public static bool GetValueForCurrentState(List<Point> state, int[,] map, out double attackValue, bool mustDefend = false)
    {
        attackValue = 0;
        var opened = BetaGetOpenedSideCount(state, map);

        var emptySpaceCount = state.Count(x => x.EmptySpace);
        var stateCount = state.Count - emptySpaceCount;

        var minusVal = -(emptySpaceCount * 100);

        if (Game.WinCount == stateCount)
        {
            attackValue = 10000 + minusVal;
            return true;
        }

        // 4
        if(Game.WinCount - 1 == stateCount)
        {
            if (opened == 1)
            {
                if (mustDefend)
                {
                    attackValue += 100;
                    return false;
                }
                attackValue += 3000 + minusVal;
                return true;
            }
            if (opened == 2)
            {
                if (mustDefend)
                {
                    attackValue += 500;
                    return false;
                }
                attackValue += 7500 + minusVal;
                return true;
            }
        }
        else if (Game.WinCount - 2 == stateCount)
        {
            /*if (opened == 1)
            {
                if (mustDefend)
                {
                    attackValue += 100;
                    return true;
                }
                attackValue += 1000;
                return true;
            }*/
            
            if (opened == 2)
            {
                if (mustDefend)
                {
                    attackValue += 300;
                    return true;
                }
                attackValue += 4200 + minusVal;
                return true;
            }
        }
        
        if (opened == 2) attackValue += stateCount * 10;
        //if (opened == 1) attackValue += stateCount * 5;
        
        return false;
    }

    public static int BetaGetOpenedSideCount(List<Point> points, int[,] map)
    {
        
        if (points.Count == 1) return 2;

        var anyEmpty = points.Any(x => x.EmptySpace);

        if(points.ElementAt(0).Value == 0) points.RemoveAt(0);
        if (points.ElementAt(points.Count -1 ).Value == 0) points.RemoveAt(points.Count - 1);

        if (points.Count == 1) return 2;
        int openedCount = 0;

        var lastP = points.Last();
        var firstP = points.First();

        var secondPointFromStart = points.ElementAt(1);
        var secondPointFromEnd = points.ElementAt(points.Count - 2);

        var firstDiffX = firstP.X - secondPointFromStart.X;
        var firstDiffY = firstP.Y - secondPointFromStart.Y;

        var Fx = firstP.X + firstDiffX;
        var Fy = firstP.Y + firstDiffY;

        var lastDiffX = lastP.X - secondPointFromEnd.X;
        var lastDiffY = lastP.Y - secondPointFromEnd.Y;

        var Lx = lastP.X + lastDiffX;
        var Ly = lastP.Y + lastDiffY;

        if (Fx >= 0 && Fy >= 0 && Fx < Game.MapSize && Fy < Game.MapSize)
            if (map[Fy, Fx] == 0) openedCount++;


        if (Lx >= 0 && Ly >= 0 && Lx < Game.MapSize && Ly < Game.MapSize)
            if (map[Ly, Lx] == 0) openedCount++;

        return openedCount;
    }

    private static bool[,] horizontalVisited = new bool[Game.MapSize, Game.MapSize];
    private static bool[,] verticalVisited = new bool[Game.MapSize, Game.MapSize];
    private static bool[,] rightDiagonalVisited = new bool[Game.MapSize, Game.MapSize];
    private static bool[,] leftDiagonalVisited = new bool[Game.MapSize, Game.MapSize];
    private static HashSet<Point> alonePoints = new HashSet<Point>();
    private static List<List<Point>> positionStates = new();

    static void BFS(int startX, int startY, int[,] map, List<List<Point>> positionStates, int player)
    {
        Queue<Point> queue = new();
        Point start = new();
        start.X = startX;
        start.Y = startY;
        start.Value = map[startY, startX];
        var neighbours = GetNeighbours(start, map, player, out var removedMirror);
        HashSet<Point> addedAlone = new();

        List<Point> outPoints = new();
        if (neighbours.Count() == 0)
        {
            if (removedMirror) return;

            outPoints.Add(start);
            positionStates.Add(outPoints);
        }

        foreach (var neighbour in neighbours) queue.Enqueue(neighbour);

        while (queue.Count > 0)
        {
            var currPoint = queue.Dequeue();

            if (IsVisited(currPoint)) continue;

            // Nalezení dalšího bodu v cestě
            if (currPoint.X + currPoint.DiffX >= 0 && currPoint.X + currPoint.DiffX < Game.MapSize && currPoint.Y + currPoint.DiffY >= 0 && currPoint.Y + currPoint.DiffY < Game.MapSize)
            {
                if (map[currPoint.Y + currPoint.DiffY, currPoint.X + currPoint.DiffX] == player || (map[currPoint.Y + currPoint.DiffY, currPoint.X + currPoint.DiffX] == 0 && !currPoint.EmptySpace))
                {
                    var p = new Point()
                    {
                        DiffX = currPoint.DiffX,
                        DiffY = currPoint.DiffY,
                        Parent = currPoint,
                        X = currPoint.X + currPoint.DiffX,
                        Y = currPoint.Y + currPoint.DiffY,
                        MoveType = currPoint.MoveType,
                        //EmptySpace = map[currPoint.Y + currPoint.DiffY, currPoint.X + currPoint.DiffX] == 0,
                        EmptySpace = !(map[currPoint.Y + currPoint.DiffY, currPoint.X + currPoint.DiffX] == player),
                        Value = map[currPoint.Y + currPoint.DiffY, currPoint.X + currPoint.DiffX]
                    };
                    queue.Enqueue(p);
                    continue;
                }
            }
            // Pokud zde není uložíme cestu
            var parent = currPoint.EmptySpace ? currPoint.Parent : currPoint;
            outPoints = new();
            if (parent == null) continue;
            
            while (parent != null)
            {
                outPoints.Add(parent);
                parent = parent.Parent;
            }

            if (outPoints.Count == 1)
            {
                alonePoints.Add(outPoints.ElementAt(0));
                continue;
            }
            foreach (var op in outPoints)
            {
                var sameP = alonePoints.FirstOrDefault(x => x.Y == op.Y && x.X == op.X);
                if (sameP != null) alonePoints.Remove(sameP);
            }
            positionStates.Add(outPoints);
        }

    }

    static IEnumerable<Point> GetNeighbours(Point currentPoint, int[,] map, int player, out bool removedMirror)
    {
        removedMirror = false;
        List<Point> allPoints = new List<Point>()
        {
            // X
            new Point(){ X = currentPoint.X + 1, Y = currentPoint.Y, Parent = currentPoint, DiffX = 1, MoveType = MoveTypes.Horizontal, EmptySpace = currentPoint.EmptySpace},
            new Point(){ X = currentPoint.X - 1, Y = currentPoint.Y, Parent = currentPoint, DiffX = -1, MoveType = MoveTypes.Horizontal, EmptySpace = currentPoint.EmptySpace},

            //Y
            new Point(){ X = currentPoint.X, Y = currentPoint.Y + 1, Parent = currentPoint, DiffY = 1, MoveType =  MoveTypes.Vertical, EmptySpace = currentPoint.EmptySpace},
            new Point(){ X = currentPoint.X, Y = currentPoint.Y - 1, Parent = currentPoint,  DiffY = -1, MoveType = MoveTypes.Vertical, EmptySpace = currentPoint.EmptySpace},

            // Dia
            new Point(){ X = currentPoint.X - 1, Y = currentPoint.Y - 1, Parent = currentPoint, DiffX = -1, DiffY = -1,MoveType = MoveTypes.LeftDiagonal, EmptySpace = currentPoint.EmptySpace},
            new Point(){ X = currentPoint.X + 1, Y = currentPoint.Y + 1, Parent = currentPoint, DiffX = 1, DiffY = 1,MoveType = MoveTypes.LeftDiagonal, EmptySpace = currentPoint.EmptySpace},

            // Dia
            new Point(){ X = currentPoint.X - 1, Y = currentPoint.Y + 1, Parent = currentPoint, DiffX = -1, DiffY = 1,MoveType = MoveTypes.RightDiagonal, EmptySpace = currentPoint.EmptySpace},
            new Point(){ X = currentPoint.X + 1, Y = currentPoint.Y - 1, Parent = currentPoint, DiffX = 1, DiffY = -1,MoveType = MoveTypes.RightDiagonal, EmptySpace = currentPoint.EmptySpace},
        };


        bool isOk;
        int mirroredX;
        int mirroredY;

        List<Point> outPoints = new();

        foreach (var p in allPoints)
        {
            isOk = p.X >= 0 && p.X < Game.MapSize && p.Y >= 0 && p.Y < Game.MapSize && (map[p.Y, p.X] == 0 || map[p.Y, p.X] == player);

            if (!isOk) continue;

            if (map[p.Y, p.X] == 0) p.EmptySpace = true;

            // Má bod zrcadlo
            mirroredX = p.X - (p.DiffX * 2);
            mirroredY = p.Y - (p.DiffY * 2);

            if (mirroredX >= 0 && mirroredY >= 0 && mirroredX < Game.MapSize && mirroredY < Game.MapSize)
            {
                // Je někde uprostřed
                if (map[mirroredY, mirroredX] == player)
                {
                    removedMirror = true;
                    continue;
                }
            }
            
            currentPoint.MoveType = p.MoveType;
            p.Value = map[p.Y, p.X];
            IsVisited(currentPoint);
            outPoints.Add(p);
        }

        return outPoints;
    }

    private static bool IsVisited(Point currPoint)
    {
        switch (currPoint.MoveType)
        {
            case MoveTypes.Horizontal:
                if (horizontalVisited[currPoint.Y, currPoint.X]) return true;
                horizontalVisited[currPoint.Y, currPoint.X] = true;
                break;

            case MoveTypes.Vertical:
                if (verticalVisited[currPoint.Y, currPoint.X]) return true;
                verticalVisited[currPoint.Y, currPoint.X] = true;
                break;

            case MoveTypes.RightDiagonal:
                if (rightDiagonalVisited[currPoint.Y, currPoint.X]) return true;
                rightDiagonalVisited[currPoint.Y, currPoint.X] = true;
                break;

            case MoveTypes.LeftDiagonal:
                if (leftDiagonalVisited[currPoint.Y, currPoint.X]) return true;
                leftDiagonalVisited[currPoint.Y, currPoint.X] = true;
                break;

            // první bod
            default:
                break;
        }
        return false;
    }

    private static void ResetVisited()
    {
        Array.Clear(horizontalVisited);
        Array.Clear(verticalVisited);
        Array.Clear(rightDiagonalVisited);
        Array.Clear(leftDiagonalVisited);
        positionStates.Clear();
        alonePoints.Clear();
    }
}

static public class TranspositionTable
{
    static Dictionary<string, double> transpositionTable = new Dictionary<string, double>();
    static public bool DoesPosExists(int[,] map, int depth, bool isMaximalizer, out double? storedValue)
    {
        storedValue = null;
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < Game.MapSize; y++) for (int x = 0; x < Game.MapSize; x++) sb.Append(map[y, x]);
        var positionString = $"{sb.ToString()}|{isMaximalizer}|{depth}";
        if (!transpositionTable.TryGetValue(positionString, out var value))
        {
            var eval = AI.CalculateCurrentPosition(map, isMaximalizer, out var outDef);
            transpositionTable.Add(positionString, eval);
            return false;
        }
        storedValue = value;
        return true;
    }
    public static void ClearTable() => transpositionTable.Clear();
}

public class Point : PointBase
{
    public Point Parent { get; set; }
    public int DiffX { get; set; } = 0;
    public int DiffY { get; set; } = 0;
    public MoveTypes MoveType { get; set; }

    public bool EmptySpace { get; set; }
    public int Value { get; set; }
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


public enum MoveTypes
{
    Horizontal,
    Vertical,
    RightDiagonal,
    LeftDiagonal
}