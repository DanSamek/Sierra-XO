using Microsoft.AspNetCore.Mvc;
using SierraXOWeb.Models;
using System.Diagnostics;
using TicTacToeAI;

namespace SierraXOWeb.Controllers
{
    public class HomeController : Controller
    {

        private object _lock = new object();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/move")]
        public IActionResult GetAiMove([FromBody] MoveSettings moveSettings)
        {
            lock (_lock)
            {
                AIMove move = new();
                Game.MapSize = moveSettings.MapSize;
                Game.WinCount = moveSettings.WinCount;
                int[,] dimmap = new int[moveSettings.MapSize, moveSettings.MapSize];

                for (int y = 0; y < moveSettings.MapSize; y++)
                    for (int x = 0; x < moveSettings.MapSize; x++)
                        dimmap[y, x] = moveSettings.Map[y][x];
                try
                {
                    AI.Depth = moveSettings.Depth;
                    AI.GetAIMove(dimmap, out int X, out int Y);
                    move.X = X;
                    move.Y = Y;
                    return Json(move);
                }
                catch (Exception)
                {
                    move.X = -1;
                    move.Y = -1;
                    return Json(move);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class MoveSettings
        {
            public int MapSize { get; set; }
            public int Depth { get; set; } = 3;
            public int WinCount { get; set; } = 8;
            public int [][] Map { get; set; }
        }
    }
}