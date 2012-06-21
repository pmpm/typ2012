using System;
using System.Collections.Generic;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models.Repositories
{
    public class GameRepository : IGameRepository
    {
        #region game repository

        public Game GetGame(Guid id)
        {
            return (Game)FnHibernateHelper.InTransaction(session => session.Get<Game>(id));
        }

        public IEnumerable<Game> GetAllGames()
        {
            return (IEnumerable<Game>)FnHibernateHelper.InTransaction(session => session.QueryOver<Game>().OrderBy(g => g.GameStartDate).Asc.List());
        }

        public IEnumerable<Game> GetFinishedGames()
        {
            return (IEnumerable<Game>)FnHibernateHelper.InTransaction(session => session.QueryOver<Game>().Where(g=>g.IsFinished).OrderBy(g => g.GameStartDate).Asc.List());
        }

        public void SaveGame(Game game)
        {
            FnHibernateHelper.InTransaction(session => session.Save(game));
        }

        public void UpdateGame(Game game)
        {
            FnHibernateHelper.InTransaction(session =>
            {
                var originalGame = session.Get<Game>(game.Id);

                originalGame.HomeTeam = game.HomeTeam;
                originalGame.GuestTeam = game.GuestTeam;
                originalGame.GameStartDate = game.GameStartDate;
                originalGame.IsScoreMultiplier = game.IsScoreMultiplier;

                session.Update(originalGame);
            });
        }

        public void FinishGame(Game game)
        {
            FnHibernateHelper.InTransaction(session =>
            {
                var resultId = session.Save(game.Result);
                var originalGame = session.Get<Game>(game.Id);
                originalGame.Result = session.Get<Result>(resultId);
                originalGame.IsFinished = true;

                session.Update(originalGame);
            });
        }

        public void UpdateResult(Result result)
        {
            FnHibernateHelper.InTransaction(session => session.Update(result));
        }

        public void DeleteGame(Game game)
        {
            this.DeleteGameBets(game.Id);

            FnHibernateHelper.InTransaction(session =>
            {
                var result = game.Result;
                session.Delete(game);
                if (result != null)
                    session.Delete(result);
            });
        }

        private void DeleteGameBets(Guid gameId)
        {
            FnHibernateHelper.InTransaction(session =>
            {
                var bets = session.QueryOver<Bet>().Where(x => x.Game.Id == gameId).List();
                foreach (var bet in bets)
                {
                    session.Delete(bet);
                }
            });
        }

        #endregion

        public IEnumerable<Team> GetAllTeams()
        {
            return (IEnumerable<Team>)FnHibernateHelper.InTransaction(session => session.QueryOver<Team>().List());
        }

        public Team GetTeam(Guid id)
        {
            return (Team)FnHibernateHelper.InTransaction(session => session.Get<Team>(id));
        }

        public void SaveTeam(Team team)
        {
            FnHibernateHelper.InTransaction(session => session.Save(team));
        }
    }
}