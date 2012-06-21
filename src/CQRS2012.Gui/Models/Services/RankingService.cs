using System;
using System.Collections.Generic;
using CQRS2012.Gui.Models.Repositories;

namespace CQRS2012.Gui.Models.Services
{
    public interface IRankingService
    {
        IEnumerable<UserTotalScores> GetMainRanking();
        void UpdateRanking();
        void SetupUserRanking(string userName);
    }

    public class RankingService : IRankingService
    {
        private readonly IRankingRepository _rankingRepository;

        public RankingService(IRankingRepository rankingRepository)
        {
            if(rankingRepository == null) 
                throw new ArgumentNullException("rankingRepostiory");

            this._rankingRepository = rankingRepository;
        }

        public IEnumerable<UserTotalScores> GetMainRanking()
        {
            return this._rankingRepository.GetMainRanking();
        }

        public void UpdateRanking()
        {
            this._rankingRepository.UpdateRanking();
        }

        public void SetupUserRanking(string userName)
        {
            if (!this._rankingRepository.IsCreated(userName))
            {
                this._rankingRepository.Create(new UserTotalScores { UserName = userName, TotalScore = 0 });
                this._rankingRepository.UpdateRanking();
            }
        }
    }
}