using System.Diagnostics;
using System.Text;

namespace TicTacToeAI;
public class AI
{
    public static int Depth = 5;
    public static double GetAIMove(int[,] map)
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
        Console.WriteLine($"{stopwatch.ElapsedMilliseconds / (double)1000} s");
        map[bestX, bestY] = -1;
        TranspositionTable.ClearTable();
        return maxValue;
    }

    static double Minimax(int[,] map, int depth, bool isMaximalizer, double alpha, double beta)
    {
        if (TranspositionTable.DoesPosExists(map, depth, isMaximalizer, out double? storedValue)) return storedValue.Value;

        if (Game.CheckWin(map, out int player))
        {
            int outValue = player == 1 ? -10 : 10;
            return outValue * depth;
        }

        if (depth == 0)
        {
            var outValue = CalculateCurrentPosition(map, isMaximalizer);
            return outValue;
        }
        if (!Game.SomethingToPlay(map)) return 0;
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

        var avg = Math.Round(evalPoints.Average(x => x.Eval), 2);

        // for maximalizer
        if (isMaximalizer)
        {
            double maxValue = int.MinValue;
            int possibleMaxValue = (depth - 1) * 10;
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


    static HashSet<int[]> PositionsToCheck(int[,] map)
    {
        HashSet<int[]> optimalPositions = new HashSet<int[]>();
        // Vybírat tahy přímo připojené + 1 
        for (int y = 0; y < Game.MapSize; y++)
        {
            for (int x = 0; x < Game.MapSize; x++)
            {
                if (map[y, x] != 0)
                {
                    for (int i = 1; i < 3; i++)
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


    public static double CalculateCurrentPosition(int[,] map, bool isMaximalizer)
    {
        // AI = Maximalizer
        var player = isMaximalizer ? -1 : 1;
        List<List<Point>> positionStates = new();
        for (int y = 0; y < Game.MapSize; y++) for (int x = 0; x < Game.MapSize; x++) if (map[y, x] == player) BFS(x, y, map, positionStates, player);

        var sortedBySize = positionStates.OrderByDescending(x => x.Count).ToList();
        for (int i = 0; i < sortedBySize.Count; i++)
        {
            foreach (var itemToCheck in sortedBySize.ToList())
            {
                if (itemToCheck == sortedBySize[i]) continue;

                bool allSame = true;
                foreach (var currChecked in itemToCheck)
                {
                    if (!sortedBySize[i].Any(x => x.X == currChecked.X && x.Y == currChecked.Y))
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
        bool[,] globalVisited = new bool[Game.MapSize, Game.MapSize];

        foreach (var state in sortedBySize) finalValue += state.Count / (double)10;

        finalValue = isMaximalizer ? (finalValue) : -(finalValue);
        return Math.Round(finalValue, 2);
    }

    static void BFS(int startX, int startY, int[,] map, List<List<Point>> positionStates, int player)
    {
        Queue<Point> queue = new();
        Point start = new();
        start.X = startX;
        start.Y = startY;
        bool[,] visited = new bool[Game.MapSize, Game.MapSize];
        var neighbours = GetNeighbours(start, visited, map, player);


        List<Point> outPoints = new();
        if (neighbours.Count() == 0)
        {
            outPoints.Add(start);
            positionStates.Add(outPoints);
        }

        foreach (var neighbour in neighbours) queue.Enqueue(neighbour);

        while (queue.Count > 0)
        {
            var currPoint = queue.Dequeue();
            visited[currPoint.Y, currPoint.X] = true;

            // Nalezení dalšího bodu v cestě
            if (currPoint.X + currPoint.DiffX >= 0 && currPoint.X + currPoint.DiffX < Game.MapSize && currPoint.Y + currPoint.DiffY >= 0 && currPoint.Y + currPoint.Y < Game.MapSize)
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
        var possiblePoints = allPoints.Where(p => p.X >= 0 && p.X < Game.MapSize && p.Y >= 0 && p.Y < Game.MapSize && !visited[p.Y, p.X] && map[p.Y, p.X] == player).ToList();

        return possiblePoints;
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
            var eval = AI.CalculateCurrentPosition(map, isMaximalizer);
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