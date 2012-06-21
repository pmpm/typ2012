using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Database;
using CQRS2012.Gui.Models.Repositories;
using CQRS2012.Gui.Models.Services;
using CQRS2012.Gui.ViewModels;

namespace CQRS2012.Gui.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SettingsController : Controller
    {
        private readonly IGameService _gameService;
        private readonly IBetStrategyService _betStrategyService;

        public SettingsController()
        {
        }

        public SettingsController(IGameService gameService, IBetStrategyService betStrategyService)
        {
            this._gameService = gameService;
            this._betStrategyService = betStrategyService;
        }

        //
        // GET: /Settings/

        public ActionResult Index()
        {
            var gamesGroupedByDate = this._gameService.GetGamesGroupByDate();

            var gamesByDates = new List<GamesByDateViewModel>(gamesGroupedByDate.Select(x => new GamesByDateViewModel { GamesDate = x.Key, Games = x.ToList() }));

            return View(gamesByDates);
        }

        public ActionResult CreateTeam()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTeam(Team team)
        {
            if (ModelState.IsValid)
            {
                this._gameService.SaveTeam(team);
                return RedirectToAction("Index");
            }
         
            return View(team);
        }

        public ActionResult Create()
        {
            this.FillTeamsLists();
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "HomeTeam,GuestTeam")] Game game, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                this._gameService.SaveGame(game, Guid.Parse(form["HomeTeam"]), Guid.Parse(form["GuestTeam"]));
                this._betStrategyService.ApplyStrategyToNewGame(game);
                return RedirectToAction("Index");
            }

            this.FillTeamsLists(form["HomeTeam"], form["GuestTeam"]);
            return View(game);
        }

        //
        // GET: /Settings/Edit
        //TODO: do przerobienia 
        public ActionResult Edit(Guid id)
        {
            var game = this._gameService.GetGame(id);

            this.FillTeamsLists(game.HomeTeam.Id.ToString(), game.GuestTeam.Id.ToString());
            return View(game);
        }

        //
        // POST: /Settings/Edit
        //TODO: do przerobienia 
        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "HomeTeam,GuestTeam")] Game game, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                this._gameService.UpdateGame(game, Guid.Parse(form["HomeTeam"]), Guid.Parse(form["GuestTeam"]));
                return RedirectToAction("Index");
            }

            this.FillTeamsLists(form["HomeTeam"], form["GuestTeam"]);
            return View(game);
        }

        //
        // GET: /Settings/Finish

        public ActionResult Finish(Guid id)
        {
            var game = this._gameService.GetGame(id);

            //TODO: popup ze nie mozna jeszcze zrobic finisha albo zablokowany button, finsh moze byc zrobiony 2 godziny po starcie gry)
            if (!this._gameService.CanFinishGame(game))
                return RedirectToAction("Index");

            return View(game);
        }

        //
        // POST: /Settings/Finish

        [HttpPost]
        public ActionResult Finish(Game game)
        {
            if (ModelState.IsValid)
            {
                this._gameService.FinishGame(game);
                return RedirectToAction("Index");
            }

            return View(game);
        }

        //
        // GET: /Settings/UpdateResult

        public ActionResult UpdateResult(Guid id)
        {
            var game = this._gameService.GetGame(id);

            return View(game);
        }

        //
        // POST: /Settings/UpdateResult

        [HttpPost]
        public ActionResult UpdateResult(Game game)
        {
            if (ModelState.IsValid)
            {
                this._gameService.UpdateGameResult(game);
                return RedirectToAction("Index");
            }

            return View(game);
        }

        // GET: /Settings/Delete/5

        public ActionResult Delete(Guid id)
        {
            var game = this._gameService.GetGame(id);

            return View(game);
        }

        //
        // POST: /Settings/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            this._gameService.DeleteGame(id);

            return RedirectToAction("Index");
        }

        private void FillTeamsLists(string selectedHomeTeamId, string selectedGuestTeamId)
        {
            var teams = this._gameService.GetAllTeams();
            ViewBag.HomeTeam = new SelectList(teams, "Id", "Name", selectedHomeTeamId);
            ViewBag.GuestTeam = new SelectList(teams, "Id", "Name", selectedGuestTeamId);
        }

        private void FillTeamsLists()
        {
            var teams = this._gameService.GetAllTeams();
            ViewBag.HomeTeam = new SelectList(teams, "Id", "Name");
            ViewBag.GuestTeam = new SelectList(teams, "Id", "Name");
        }
    }
}
