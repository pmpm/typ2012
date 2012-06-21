using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Services;
using CQRS2012.Gui.ViewModels;

namespace CQRS2012.Gui.Controllers
{
    [Authorize]
    public class TableController : Controller
    {
        private readonly IRankingService _rankingService;
        private readonly IGameService _gameService;

        public TableController(IRankingService rankingService, IGameService gameService)
        {
            this._rankingService = rankingService;
            this._gameService = gameService;
        }

        //
        // GET: /Table/

        public ActionResult Index()
        {
            var tabledata = this._gameService.GetDateForTable();
            var mainRanking = (List<UserTotalScores>)this._rankingService.GetMainRanking();
            var usersWithPosition = mainRanking.GroupBy(x => x.UserName).ToDictionary(x=>x.Key,x=>x.ToList().SingleOrDefault());

            ViewBag.LastPosition = mainRanking[mainRanking.Count - 1].Position;
            ViewBag.UsersWithPosition = usersWithPosition;
            ViewBag.Data = tabledata;
            ViewBag.Users = tabledata.Keys;
            var el = tabledata.FirstOrDefault();
            ViewBag.Games = el.Value.Keys;
            ViewBag.CanBetGame = el.Value.Keys.ToDictionary(game => game, game => this._gameService.CanBetGame(game.GameStartDate));

         
            return View();
        }

    }
}
