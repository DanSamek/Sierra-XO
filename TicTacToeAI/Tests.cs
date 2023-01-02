﻿namespace TicTacToeAI;
public static class Tests
{
    public static void EvalutationTests()
    {
        int[,] testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };

        Game.MapSize = 5;
        var eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0);

        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,0,-1,0,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };

        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.1);

        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,-1,-1,0,0 },
            { 0,0,-1,0,0 },
            { 0,0,0,0,0 }
        };
        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.6);

        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,-1,-1,-1,0 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };
        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.3);


        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,-1,-1,-1,-1 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };
        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.4);

        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { 0,-1,-1,-1,0 },
            { 0,0,-1,0,0 },
            { 0,0,0,0,0 }
        };
        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.9);


        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 0,0,0,0,0 },
            { -1,-1,-1,-1,-1 },
            { 0,0,0,0,0 },
            { 0,0,0,0,0 }
        };
        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 0.5);



        testMap = new int[5, 5]
        {
            { 0,0,0,0,0 },
            { 1,0,0,0,0 },
            { 1,-1,-1,-1,1 },
            { -1,0,0,0,0 },
            { 0,0,0,0,0 }
        };
        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 1);

        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {-1,0,0,0,0},
            {-1,1,1,1,-1},
            {0,0,1,0,0},
            {0,0,0,0,0}
        };
        eval = AI.CalculateCurrentPosition(testMap, false);
        AreEqual(eval, -0.9+-0.3);


        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {-1,0,1,0,0},
            {-1,1,1,1,-1},
            {0,0,1,0,0},
            {0,0,0,0,0}
        };
        eval = AI.CalculateCurrentPosition(testMap, false);
        AreEqual(eval, -1.7);

        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {1,0,-1,0,0},
            {1,-1,-1,-1,1},
            {0,0,-1,0,0},
            {0,0,0,0,0}
        };
        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 1.7);

        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {1,0,0,-1,0},
            {1,-1,-1,-1,0},
            {0,0,-1,0,0},
            {0,0,0,0,0}
        };
        eval = AI.CalculateCurrentPosition(testMap, true);
        AreEqual(eval, 1.5);
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
        Game.MapSize = 5;
        Game.WinCount = 4;
        var eval = AI.GetAIMove(testMap);
        Game.DrawMap(testMap);
        AreEqual(eval, 10);


        testMap = new int[5, 5]
        {
            {0,0,0,0,0},
            {1,0,0,0,0},
            {0,0,-1,0,0},
            {0,-1,-1,-1,0},
            {0,0,-1,0,0}
        };
        Game.Depth = 5;
        eval = AI.GetAIMove(testMap);
        Game.DrawMap(testMap);
        AreEqual(eval, 10);


        testMap = new int[7, 7]
        {
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,1,0,0},
            {0,1,-1,-1,0,0,0},
            {-1,1,-1,-1,-1,0,0},
            {0,0,0,-1,0,0,0},
            {0,0,0,0,0,0,0}
        };
        Game.Depth = 5;
        Game.MapSize = 7;
        Game.WinCount = 5;
        eval = AI.GetAIMove(testMap);
        Game.DrawMap(testMap);
        AreEqual(eval, 10);

        testMap = new int[7, 7]
        {
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,1,1,1,1},
            {0,0,1,0,0,0,0},
            {0,0,1,0,0,0,0},
            {0,0,1,0,0,0,0},
            {0,0,1,0,0,0,0}
        };
        
        Game.WinCount = 5;
        Game.Depth = 5;
        Game.MapSize = 7;
        eval = AI.GetAIMove(testMap);
        Game.DrawMap(testMap);
        AreEqual(eval, 0.8);

    }
}
