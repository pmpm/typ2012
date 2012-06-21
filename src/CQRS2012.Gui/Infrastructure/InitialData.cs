using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Database;
using CQRS2012.Gui.Models.Services;

namespace CQRS2012.Gui.Infrastructure
{
    public class InitialData
    {
        private readonly IRankingService _rankingService;
        private readonly IBetStrategyService _betStrategyService;

        public InitialData(IRankingService rankingService, IBetStrategyService betStrategyService)
        {
            _rankingService = rankingService;
            _betStrategyService = betStrategyService;
        }

        private List<Team> teams = new List<Team>
        {
            new Team {Name = "Polska", PathToFlag = "/Content/Images/pl.png"},
            new Team {Name = "Rosja", PathToFlag = "/Content/Images/ru.png"},
            new Team {Name = "Holandia", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Grecja", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Czechy", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Dania", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Portugalia", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "W這chy", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Chorwacja", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Anglia", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Szwecja", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Niemcy", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Hiszpania", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Irlandia", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Francja", PathToFlag = "/Content/Images/ne.png"},
            new Team {Name = "Ukraina", PathToFlag = "/Content/Images/ne.png"},
        };

        public void Setup()
        {
          
            var users = Membership.GetAllUsers();
            var teamsCount = (int)FnHibernateHelper.InTransaction(session => session.QueryOver<Team>().RowCount());
            var gamesCount = (int)FnHibernateHelper.InTransaction(session => session.QueryOver<Game>().RowCount());
            var userTotalScoresCount = (int)FnHibernateHelper.InTransaction(session => session.QueryOver<UserTotalScores>().RowCount());

            if (teamsCount == 0) this.SaveTeams();
            if (gamesCount == 0) this.SaveGames();
            if (users.Count == 0) this.CreateAdminUser();
            if (userTotalScoresCount == 0) this.SaveUserTotalScores();
            if (userTotalScoresCount == 0 && users["randomer"] != null)
                this.SetRandomerStrategyForUser("randomer");
        }

        private void CreateAdminUser()
        {
            var userName = "admin";
            var pass = "xxxxxx";

            Membership.CreateUser(userName, pass, "admin@sd.pl");
            Roles.CreateRole("Administrator");
            Roles.AddUserToRole("admin", "Administrator");
        }

        private void SetRandomerStrategyForUser(string userName)
        {
            var strategyName = "Randomer";

            this._betStrategyService.BindUserWithStrategy(userName, strategyName);
            this._betStrategyService.ApplyStrategyForUserToExistedGames(userName, strategyName);
            this._rankingService.SetupUserRanking(userName);
        }

        private void SaveUserTotalScores()
        {
            var users = Membership.GetAllUsers();

            FnHibernateHelper.InTransaction(session =>
            {
                foreach (MembershipUser user in users)
                {
                    session.Save(new UserTotalScores { UserName = user.UserName, TotalScore = 0, Position = 1 });
                }
            });
        }

        private void SaveTeams()
        {
            FnHibernateHelper.InTransaction(session =>
            {
                foreach (var team in teams)
                {
                    session.Save(team);
                }
            });
        }

        private void SaveGames()
        {
            var allTeams = (IEnumerable<Team>)FnHibernateHelper.InTransaction(session => session.QueryOver<Team>().List());

            var teamsCollection = allTeams.ToDictionary(team => team.Name);

            var groupsGames = new List<Game>
            {
                new Game{ HomeTeam = teamsCollection["Polska"], GuestTeam = teamsCollection["Grecja"],GameStartDate = DateTime.Parse("2012-06-08 18:00")},
                new Game{ HomeTeam = teamsCollection["Rosja"], GuestTeam = teamsCollection["Czechy"],GameStartDate = DateTime.Parse("2012-06-08 20:45")},
                new Game{ HomeTeam = teamsCollection["Holandia"], GuestTeam = teamsCollection["Dania"], GameStartDate = DateTime.Parse("2012-06-09 18:00") },
                new Game{ HomeTeam = teamsCollection["Niemcy"], GuestTeam = teamsCollection["Portugalia"], GameStartDate = DateTime.Parse("2012-06-09 20:45") },
                new Game{ HomeTeam = teamsCollection["Hiszpania"], GuestTeam = teamsCollection["W這chy"], GameStartDate = DateTime.Parse("2012-06-10 18:00") },
                new Game{ HomeTeam = teamsCollection["Irlandia"], GuestTeam = teamsCollection["Chorwacja"], GameStartDate = DateTime.Parse("2012-06-10 20:45") },
                new Game{ HomeTeam = teamsCollection["Francja"], GuestTeam = teamsCollection["Anglia"], GameStartDate = DateTime.Parse("2012-06-11 18:00") },
                new Game{ HomeTeam = teamsCollection["Ukraina"], GuestTeam = teamsCollection["Szwecja"], GameStartDate = DateTime.Parse("2012-06-11 20:45") },

                new Game{ HomeTeam = teamsCollection["Grecja"], GuestTeam = teamsCollection["Czechy"], GameStartDate = DateTime.Parse("2012-06-12 18:00") },
                new Game{ HomeTeam = teamsCollection["Polska"], GuestTeam = teamsCollection["Rosja"], GameStartDate = DateTime.Parse("2012-06-12 20:45") },
                new Game{ HomeTeam = teamsCollection["Dania"], GuestTeam = teamsCollection["Portugalia"], GameStartDate = DateTime.Parse("2012-06-13 18:00") },
                new Game{ HomeTeam = teamsCollection["Holandia"], GuestTeam = teamsCollection["Niemcy"], GameStartDate = DateTime.Parse("2012-06-13 20:45") },
                new Game{ HomeTeam = teamsCollection["W這chy"], GuestTeam = teamsCollection["Chorwacja"], GameStartDate = DateTime.Parse("2012-06-14 18:00") },
                new Game{ HomeTeam = teamsCollection["Hiszpania"], GuestTeam = teamsCollection["Irlandia"], GameStartDate = DateTime.Parse("2012-06-14 20:45") },
                new Game{ HomeTeam = teamsCollection["Szwecja"], GuestTeam = teamsCollection["Anglia"], GameStartDate = DateTime.Parse("2012-06-15 18:00") },
                new Game{ HomeTeam = teamsCollection["Ukraina"], GuestTeam = teamsCollection["Francja"], GameStartDate = DateTime.Parse("2012-06-15 20:45") },

                new Game{ HomeTeam = teamsCollection["Czechy"], GuestTeam = teamsCollection["Polska"], GameStartDate = DateTime.Parse("2012-06-16 20:45") },
                new Game{ HomeTeam = teamsCollection["Grecja"], GuestTeam = teamsCollection["Rosja"], GameStartDate = DateTime.Parse("2012-06-16 20:45") },
                new Game{ HomeTeam = teamsCollection["Portugalia"], GuestTeam = teamsCollection["Holandia"], GameStartDate = DateTime.Parse("2012-06-17 20:45") },
                new Game{ HomeTeam = teamsCollection["Dania"], GuestTeam = teamsCollection["Niemcy"], GameStartDate = DateTime.Parse("2012-06-17 20:45") },
                new Game{ HomeTeam = teamsCollection["Chorwacja"], GuestTeam = teamsCollection["Hiszpania"], GameStartDate = DateTime.Parse("2012-06-18 20:45") },
                new Game{ HomeTeam = teamsCollection["W這chy"], GuestTeam = teamsCollection["Irlandia"], GameStartDate = DateTime.Parse("2012-06-18 20:45") },
                new Game{ HomeTeam = teamsCollection["Anglia"], GuestTeam = teamsCollection["Ukraina"], GameStartDate = DateTime.Parse("2012-06-19 20:45") },
                new Game{ HomeTeam = teamsCollection["Szwecja"], GuestTeam = teamsCollection["Francja"], GameStartDate = DateTime.Parse("2012-06-19 20:45") },
            };

            FnHibernateHelper.InTransaction(session =>
                                                {
                                                    foreach (var game in groupsGames)
                                                    {
                                                        session.Save(game);
                                                    }
                                                });
        }
    }
}