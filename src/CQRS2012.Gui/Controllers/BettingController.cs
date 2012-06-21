using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Database;
using CQRS2012.Gui.Models.Repositories;
using CQRS2012.Gui.Models.Services;
using CQRS2012.Gui.ViewModels;

namespace CQRS2012.Gui.Controllers
{
    [Authorize]
    public class BettingController : Controller
    {
        private readonly IGameService _gameService;

        public BettingController(IGameRepository gameRepository, IBetRepository betRepository, IGameService gameService)
        {
            this._gameService = gameService;
        }

        //
        // GET: /Betting/

        public ActionResult Index()
        {
            var gamesWithBets = this._gameService.GetGamesWithBetsGroupByDate(User.Identity.Name);
            var gamesWithBetsVM = gamesWithBets.Select(x => new GamesWithBetsViewModel { GamesDate = x.Key, GamesWithBets = x.ToDictionary(v => v.Key, v => v.Value) });

            var gamesWithBets2 = this._gameService.GetGamesWithBetsForUser(User.Identity.Name);
            ViewBag.NotFinishedGames = gamesWithBets2.Keys.Where(x=>!x.IsFinished).OrderBy(x=>x.GameStartDate);
            ViewBag.FinishedGames = gamesWithBets2.Keys.Where(x=>x.IsFinished).OrderByDescending(x=>x.GameStartDate);
            ViewBag.CanBetGame = gamesWithBets2.Keys.ToDictionary(game => game, game => this._gameService.CanBetGame(game.GameStartDate));
            ViewBag.BetFor = gamesWithBets2;

          
            return View(gamesWithBetsVM);
        }

       
        public string UpdateBet(string id, string value)
        {
            var trimedValue = value.Replace(" ", "");

            var game = this._gameService.GetGame(Guid.Parse(id));
            var bet = this._gameService.FindBet(Guid.Parse(id), User.Identity.Name);
            if (bet == null)
                bet = new Bet { Game = game };

            //TODO: popup ze nie mozna jeszcze zrobic fi
            if (!this._gameService.CanBetGame(game.GameStartDate))
                return (bet.Id == Guid.Empty) ? "?:?" : bet.Result.HomeGoals + ":" + bet.Result.GuestGoals;

          
            var splitedString = trimedValue.Split(':');
            int homeGoals; 
            int guestGoals;
            //złe dane
            if (!int.TryParse(splitedString[0], out homeGoals) || !int.TryParse(splitedString[1], out guestGoals))
            {
                return (bet.Id == Guid.Empty) ? "?:?" : bet.Result.HomeGoals + ":" + bet.Result.GuestGoals;
            }

            //validacja
            if (homeGoals >= 100 || guestGoals >= 100 || homeGoals < 0 || guestGoals < 0)
            {
                return (bet.Id == Guid.Empty) ? "?:?" : bet.Result.HomeGoals + ":" + bet.Result.GuestGoals;
            }

            //create
            if (bet.Id == Guid.Empty)
            {
                bet.UserName = User.Identity.Name;
                bet.Result = new Result{HomeGoals = homeGoals, GuestGoals = guestGoals};
                this._gameService.SaveBet(bet);
            }
            //edit
            else
            {
                bet.Result.HomeGoals = homeGoals;
                bet.Result.GuestGoals = guestGoals;
                this._gameService.UpdateBet(bet);
            }

            return homeGoals + ":" + guestGoals;
        }

        /*
        //
        // GET: /Betting/Create

        public ActionResult Create(Guid id)
        {
            var game = this._gameService.GetGame(id);

            //TODO: popup ze nie mozna jeszcze zrobic fi
            if (!this._gameService.CanBetGame(game.GameStartDate))
                return RedirectToAction("Index");
           
            return View( new Bet { Game = game });
        }

        //
        // POST: /Betting/Create

        [HttpPost]
        public ActionResult Create(Bet bet)
        {
            var game = this._gameService.GetGame(bet.Game.Id);
            //TODO: przekierowanie do odpowiedniej strony z info ze nie mozna, a wogole to jest to odpowiedzialnosc serwisu BetService
            if (!this._gameService.CanBetGame(game.GameStartDate))
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                bet.UserName = User.Identity.Name;
                this._gameService.SaveBet(bet);
                return RedirectToAction("Index");
            }

            return View(bet);
        }

        //
        // GET: /Betting/Edytuj

        public ActionResult Edit(Guid id)
        {
            var bet = this._gameService.GetBet(id);

            //TODO: popup ze nie mozna juz zabetowac
            if (!this._gameService.CanBetGame(bet.Game.GameStartDate))
                return RedirectToAction("Index");

            return View(bet);
        }

        //
        // POST: /Betting/Edytuj

        [HttpPost]
        public ActionResult Edit(Bet bet)
        {
            var game = this._gameService.GetGame(bet.Game.Id);
            bet.Game = game;

            //TODO: popup ze nie mozna juz zabetowac
            if (!this._gameService.CanBetGame(bet.Game.GameStartDate))
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                this._gameService.UpdateBet(bet);
                return RedirectToAction("Index");
            }

           
            return View(bet);
        }

        */
    }
}
