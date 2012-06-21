using System;
using System.Collections.Generic;

namespace CQRS2012.Gui.Models.Repositories
{
    public interface IBetRepository
    {
        IEnumerable<Bet> GetAllBets();
        Bet GetBet(Guid id);
        IEnumerable<Bet> GetUserBets(string userName);
        void SaveBet(Bet bet);
        void UpdateBet(Bet bet);
        IEnumerable<Bet> GetBetsForGame(Guid gameId);
        Bet FindBet(Guid gameId, string userName);
    }
}