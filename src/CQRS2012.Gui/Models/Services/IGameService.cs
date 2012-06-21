using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRS2012.Gui.Models.Services
{
    public interface IGameService
    {
        IEnumerable<IGrouping<DateTime, Game>> GetGamesGroupByDate();
        bool SaveGame(Game game, Guid homeTeamId, Guid guestTeamId);
        bool UpdateGame(Game game, Guid homeTeamId, Guid guestTeamId);
        Game GetGame(Guid id);
        bool CanFinishGame(Game game);
        void FinishGame(Game game);
        void DeleteGame(Guid id);

        IEnumerable<IGrouping<DateTime, KeyValuePair<Game, Bet>>> GetGamesWithBetsGroupByDate(string userName);
        IDictionary<string, Dictionary<Game, Bet>> GetDateForTable();
        IList<Tuple<string, object[]>> GetDataForChart();
        IDictionary<Game, Bet> GetGamesWithBetsForUser(string userName);
        bool CanBetGame(DateTime gameStartDate);
        void SaveBet(Bet bet);
        Bet GetBet(Guid id);
        void UpdateBet(Bet bet);
        Bet FindBet(Guid gameId, string userName);

        void UpdateGameResult(Game game);

        void SaveTeam(Team team);
        IEnumerable<Team> GetAllTeams();
    }
}