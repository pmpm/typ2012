using System;
using System.Web.Security;
using CQRS2012.Gui.Models.Repositories;

namespace CQRS2012.Gui.Models.Services
{
    public interface IBetStrategyService
    {
        void ApplyStrategyToNewGame(Game game);
        void ApplyStrategyForUserToExistedGames(string userName, string strategyName);
        void BindUserWithStrategy(string userName, string strategyName);
    }

    public class BetStrategyService : IBetStrategyService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IBetRepository _betRepository;
        private readonly IStrategyRepository _strategyRepository;
        private readonly IStrategyFactory _strategyFactory;
        private readonly IRankingRepository _rankingRepository;
        private readonly IScorer _scorer;

        public BetStrategyService(IGameRepository gameRepository, IBetRepository betRepository, IStrategyRepository strategyRepository, 
            IStrategyFactory strategyFactory,IRankingRepository rankingRepository, IScorer scorer)
        {
            this._gameRepository = gameRepository;
            this._betRepository = betRepository;
            this._strategyRepository = strategyRepository;
            this._strategyFactory = strategyFactory;
            this._rankingRepository = rankingRepository;
            this._scorer = scorer;
        }

        public void BindUserWithStrategy(string userName, string strategyName)
        {
            this._strategyRepository.BindUserWithStrategy(new UserStrategy { UserName = userName, StrategyName = strategyName });
        }

        public void ApplyStrategyToNewGame(Game game)
        {
            if (!this.CanApplyStrategyForGame(game))
                return;

            var usersStrategys = this._strategyRepository.GetAllUserStrategy();
            foreach (UserStrategy userStrategy in usersStrategys)
            {
                var betStrategy = this._strategyFactory.Create(userStrategy.StrategyName);
                var result = betStrategy.CalculateResult();

                this._betRepository.SaveBet(new Bet { Game = game, Result = result, UserName = userStrategy.UserName });
            }
        }

        public void ApplyStrategyForUserToExistedGames(string userName, string strategyName)
        {
            if (!this.CheckIfUserExists(userName))
                return;
          
            var betStrategy = this._strategyFactory.Create(strategyName);
            var games = this._gameRepository.GetAllGames();

            foreach (var game in games)
            {
                var result = betStrategy.CalculateResult();
                var bet = new Bet { Game = game, Result = result, UserName = userName };
                if(game.IsFinished)
                {
                    bet.Score = this._scorer.CalculateScore(bet);
                }

                this._betRepository.SaveBet(bet);
            }

           // this._rankingRepository.UpdateRanking();
        }

        private bool CanApplyStrategyForGame(Game game)
        {
            return this._gameRepository.GetGame(game.Id) != null;
        }

        private bool CheckIfUserExists(string userName)
        {
            return Membership.GetUser(userName) != null;
        }
    }
}