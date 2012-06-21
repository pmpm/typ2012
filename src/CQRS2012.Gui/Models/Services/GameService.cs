using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using CQRS2012.Gui.Models.Repositories;

namespace CQRS2012.Gui.Models.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IBetRepository _betRepository;
        private readonly IRankingRepository _rankingRepository;
        private readonly IScorer _scorer;

        public GameService(IGameRepository gameRepository, IBetRepository betRepository, IRankingRepository rankingRepository, IScorer scorer)
        {
            if (gameRepository == null) throw new ArgumentNullException("gameRepository");
            if (betRepository == null) throw new ArgumentNullException("betRepository");
            if (scorer == null) throw new ArgumentNullException("scorer");

            this._gameRepository = gameRepository;
            this._betRepository = betRepository;
            this._rankingRepository = rankingRepository;
            this._scorer = scorer;
        }

        #region Game implementation

        public IEnumerable<IGrouping<DateTime, Game>> GetGamesGroupByDate()
        {
            var games = this._gameRepository.GetAllGames();
            return games.GroupBy(x => x.GameStartDate.Date);
        }

        public bool SaveGame(Game game, Guid homeTeamId, Guid guestTeamId)
        {
            //TODO: mozna by pokombinowac z modelbinder
            game.HomeTeam = this._gameRepository.GetTeam(homeTeamId);
            game.GuestTeam = this._gameRepository.GetTeam(guestTeamId);

            this._gameRepository.SaveGame(game);

            return true;
        }


        public bool UpdateGame(Game game, Guid homeTeamId, Guid guestTeamId)
        {
            //TODO: mozna by pokombinowac z modelbinder
            game.HomeTeam = this._gameRepository.GetTeam(homeTeamId);
            game.GuestTeam = this._gameRepository.GetTeam(guestTeamId);

            this._gameRepository.UpdateGame(game);

            if (game.IsFinished) this.SetBetsScores(game.Id);

            return true;
        }

        public Game GetGame(Guid id)
        {
            var game = this._gameRepository.GetGame(id);

            if (game == null)
                throw new Exception("game not exists");

            return game;
        }

        public bool CanFinishGame(Game game)
        {
            var timeZoneInfoPoland = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            var convertedDateTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfoPoland);

            if (game.IsFinished)
                throw new Exception("finish game");

            return game.GameStartDate.AddHours(1).AddMinutes(45) < convertedDateTimeNow;
        }

        public void FinishGame(Game game)
        {
            this._gameRepository.FinishGame(game);
            this.SetBetsScores(game.Id);
        }

        public void DeleteGame(Guid id)
        {
            var game = this.GetGame(id);

            if (game == null)
                throw new Exception("game not exists");

            this._gameRepository.DeleteGame(game);
        }

        #endregion

        #region Bet implementation

        public IEnumerable<IGrouping<DateTime, KeyValuePair<Game, Bet>>> GetGamesWithBetsGroupByDate(string userName)
        {
            var games = this._gameRepository.GetAllGames();
            var bets = this._betRepository.GetUserBets(userName);

            var gamesAndBets = new Dictionary<Game, Bet>();
            foreach (var game in games)
            {
                var bet = bets.Select(x => x).Where(x => x.Game.Id == game.Id).SingleOrDefault();
                gamesAndBets.Add(game, bet);
            }

            return gamesAndBets.OrderByDescending(x => x.Key.GameStartDate).GroupBy(x => x.Key.GameStartDate.Date).OrderByDescending(x => x.Key.Date);
        }

        public IDictionary<string, Dictionary<Game, Bet>> GetDateForTable()
        {
            var data = new Dictionary<string, Dictionary<Game, Bet>>();
            var games = (List<Game>)this._gameRepository.GetAllGames();
            var users = Membership.GetAllUsers();

            foreach (MembershipUser user in users)
            {
                if (user.IsApproved)
                {
                    data.Add(user.UserName, (Dictionary<Game, Bet>)this.GetGamesWithBets(user.UserName, games));
                }
            }

            return data;
        }

        //item1 - username, item2-scores
        public IList<Tuple<string, object[]>> GetDataForChart()
        {
            var data = new List<Tuple<string, object[]>>();
            var games = (List<Game>)this._gameRepository.GetFinishedGames();
            var users = Membership.GetAllUsers();

            foreach (MembershipUser user in users)
            {
                if (user.IsApproved)
                {
                    int increment = 0;
                    var scores = new object[games.Count+1];
                    var gamesWithBets = (Dictionary<Game, Bet>)this.GetGamesWithBets(user.UserName, games);
                    var bets = gamesWithBets.Values.ToArray();
                    scores[0] = 0; //initial point
                    for (var i = 0; i < bets.Length; i++)
                    {
                        increment += (bets[i] != null) ? bets[i].Score : 0;
                        scores[i+1] = increment;
                    }

                    data.Add(new Tuple<string, object[]>(user.UserName, scores));

                }
            }

            return data;
        }

        private IDictionary<Game, Bet> GetGamesWithBets(string userName, List<Game> games)
        {
            var bets = this._betRepository.GetUserBets(userName);

            var gamesAndBets = new Dictionary<Game, Bet>();
            foreach (var game in games)
            {
                var bet = bets.Select(x => x).Where(x => x.Game.Id == game.Id).SingleOrDefault();
                gamesAndBets.Add(game, bet);
            }

            return gamesAndBets;
        }

        public IDictionary<Game, Bet> GetGamesWithBetsForUser(string userName)
        {
            var bets = this._betRepository.GetUserBets(userName);
            var games = (List<Game>)this._gameRepository.GetAllGames();
            var gamesAndBets = new Dictionary<Game, Bet>();
            foreach (var game in games)
            {
                var bet = bets.Select(x => x).Where(x => x.Game.Id == game.Id).SingleOrDefault();
                gamesAndBets.Add(game, bet);
            }

            return gamesAndBets;
        }

        public bool CanBetGame(DateTime gameStartDate)
        {
            var timeZoneInfoPoland = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            var convertedDateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfoPoland);
            return convertedDateTime < gameStartDate.AddMinutes(-15);
        }

        public void SaveBet(Bet bet)
        {
            this._betRepository.SaveBet(bet);
        }

        public void UpdateBet(Bet bet)
        {
            this._betRepository.UpdateBet(bet);
        }

        public Bet GetBet(Guid id)
        {
            var bet = this._betRepository.GetBet(id);

            if (bet == null)
                throw new Exception("bet not exists");

            return bet;
        }

        public Bet FindBet(Guid gameId, string userName)
        {
            var bet = this._betRepository.FindBet(gameId, userName);

            return bet;
        }

        #endregion

        public void UpdateGameResult(Game game)
        {
            this._gameRepository.UpdateResult(game.Result);
            this.SetBetsScores(game.Id);
        }

        public void SaveTeam(Team team)
        {
            this._gameRepository.SaveTeam(team);
        }

        public IEnumerable<Team> GetAllTeams()
        {
            return this._gameRepository.GetAllTeams();
        }

        private void SetBetsScores(Guid gameId)
        {
            var betsForGame = this._betRepository.GetBetsForGame(gameId);

            foreach (var bet in betsForGame)
            {
                bet.Score = this._scorer.CalculateScore(bet);
                this._betRepository.UpdateBet(bet);
            }

            this._rankingRepository.UpdateRanking();
        }
    }
}