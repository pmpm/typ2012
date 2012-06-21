using System;
using System.Collections.Generic;
using System.Linq;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models.Repositories
{
    public class BetRepository : IBetRepository
    {
        public Bet GetBet(Guid id)
        {
            return (Bet)FnHibernateHelper.InTransaction(session => session.Get<Bet>(id));
        }

        public IEnumerable<Bet> GetAllBets()
        {
            return (IEnumerable<Bet>)FnHibernateHelper.InTransaction(session => session.QueryOver<Bet>().List());
        }

        public IEnumerable<Bet> GetUserBets(string userName)
        {
            return (IEnumerable<Bet>)FnHibernateHelper.InTransaction(session => session.QueryOver<Bet>().Where(x=>x.UserName == userName).List());
        }

        public Bet FindBet(Guid gameId, string userName)
        {
            return (Bet)FnHibernateHelper.InTransaction(session => session.QueryOver<Bet>()
                                                                                       .Where(x => x.UserName == userName)
                                                                                       .And(x=> x.Game.Id == gameId)
                                                                                       .SingleOrDefault());
        }

        public IEnumerable<Bet> GetBetsForGame(Guid gameId)
        {
            return (IEnumerable<Bet>)FnHibernateHelper.InTransaction(session => session.QueryOver<Bet>().Where(x=>x.Game.Id == gameId).List());
        }

        public void SaveBet(Bet bet)
        {
            FnHibernateHelper.InTransaction(session =>
                                                {
                                                    session.Save(bet.Result);
                                                    session.Save(bet);
                                                });
        }

        public void UpdateBet(Bet bet)
        {
            FnHibernateHelper.InTransaction(session =>
                                                {
                                                    var originalBet = session.Get<Bet>(bet.Id);
                                                    originalBet.Result.HomeGoals = bet.Result.HomeGoals;
                                                    originalBet.Result.GuestGoals = bet.Result.GuestGoals;
                                                    originalBet.Score = bet.Score;

                                                    session.Update(originalBet.Result);
                                                });
        }
    }
}