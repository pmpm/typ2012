using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using CQRS2012.Gui.Models.Database;
using CQRS2012.Gui.Models.Services;

namespace CQRS2012.Gui.Models.Repositories
{
    public interface IRankingRepository
    {
        IEnumerable<UserTotalScores> GetMainRanking();
        void Create(UserTotalScores userTotalScores);
        void Update(UserTotalScores userTotalScores);
        bool IsCreated(string userName);
        UserTotalScores GetRankingForUser(string userName);
        void UpdateRanking();
    }

    public class RankingRepository : IRankingRepository
    {
        private readonly IBetRepository _betRepository;
        private readonly IScorer _scorer;

        public RankingRepository(IBetRepository betRepository, IScorer scorer)
        {
            this._betRepository = betRepository;
            this._scorer = scorer;
        }

        public void Create(UserTotalScores userTotalScores)
        {
            FnHibernateHelper.InTransaction(session => session.Save(userTotalScores));
        }

        public void Update(UserTotalScores userTotalScores)
        {
            FnHibernateHelper.InTransaction(session => session.Update(userTotalScores));
        }

        public bool IsCreated(string userName)
        {
            var rowCount = (int)FnHibernateHelper.InTransaction(session => session.QueryOver<UserTotalScores>().Where(x => x.UserName == userName).RowCount());

            return (rowCount != 0) ? true : false;
        }       
        
        public UserTotalScores GetRankingForUser(string userName)
        {
            return (UserTotalScores)FnHibernateHelper.InTransaction(session => session.QueryOver<UserTotalScores>().Where(x => x.UserName == userName).SingleOrDefault());
        }

        public IEnumerable<UserTotalScores> GetMainRanking()
        {
            var userRanking = (IEnumerable<UserTotalScores>)FnHibernateHelper.InTransaction(session => session.QueryOver<UserTotalScores>().OrderBy(x => x.Position).Asc.List());
           
            this.DeleteFromRankingBlockedUser(ref userRanking);

            return userRanking;
        }

        public void UpdateRanking()
        {
            var users = Membership.GetAllUsers();
            var bets = this._betRepository.GetAllBets();
            var betGroupByUser = bets.GroupBy(x => x.UserName).ToDictionary(x => x.Key, x => x.ToList());

            //add users to list which haven't had any bet yet)
            foreach (MembershipUser user in users)
            {
                if(!betGroupByUser.ContainsKey(user.UserName))
                {
                    betGroupByUser.Add(user.UserName, new List<Bet>());
                }
            }

            this.DeleteFromRankingBlockedUser(ref betGroupByUser);

            var usersWithTotalScoresAndPositions = this._scorer.CalculatePositions(betGroupByUser);

            foreach (var item in usersWithTotalScoresAndPositions)
            {
                var userTotalScore = this.GetRankingForUser(item.Item1);
                userTotalScore.TotalScore = item.Item2;
                userTotalScore.Position = item.Item3;

                this.Update(userTotalScore);
            }
        }

        private void DeleteFromRankingBlockedUser(ref IEnumerable<UserTotalScores> rankingList)
        {
            var users = Membership.GetAllUsers();

            foreach (MembershipUser user in users)
            {
                if (!user.IsApproved)
                {
                    var elementToDelete = rankingList.Where(x => x.UserName == user.UserName).SingleOrDefault();
                    ((List<UserTotalScores>)rankingList).Remove(elementToDelete);
                }
            }
        }

        private void DeleteFromRankingBlockedUser(ref Dictionary<string, List<Bet>> betGroupByUser)
        {
            var users = Membership.GetAllUsers();

            foreach (MembershipUser user in users)
            {
                if (!user.IsApproved)
                {
                    betGroupByUser.Remove(user.UserName);
                }
            }
        }
    }
}