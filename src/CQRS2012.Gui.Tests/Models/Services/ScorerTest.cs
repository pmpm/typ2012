using System;
using System.Collections.Generic;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Services;
using NUnit.Framework;

namespace CQRS2012.Gui.Tests.Models.Services
{
    [TestFixture]
    public class ScorerTest
    {
        private Scorer _scorer;
        private Game _game;
        private Bet _bet;

        [SetUp]
        public void Setup()
        {
            this._scorer = new Scorer();
            this._game = new Game
                        {
                            Result = new Result { HomeGoals = 1, GuestGoals = 1 },
                            HomeTeam = new Team(),
                            GuestTeam = new Team(),
                            GameStartDate = DateTime.Now,
                        };

            this._bet = new Bet { Game = _game };
        }

        private List<Result> sampleResults = new List<Result>
                                       {
                                           new Result { HomeGoals = 1, GuestGoals = 1 },
                                           new Result { HomeGoals = 2, GuestGoals = 0 },
                                           new Result { HomeGoals = 3, GuestGoals = 7 },
                                           new Result { HomeGoals = 12, GuestGoals = 10 },
                                           new Result { HomeGoals = 0, GuestGoals = 0 },
                                       };

        [Test]
        public void CalculateScore_should_return_proper_value_if_user_type_exact_result([ValueSource("sampleResults")] Result sampleResult)
        {
            var expectedScore = (int)Score.ExactResult;
            this._bet.Game.Result = sampleResult;
            this._bet.Result = sampleResult;

            var resultScore = this._scorer.CalculateScore(this._bet);

            Assert.That(resultScore, Is.EqualTo(expectedScore));
        }

        private Dictionary<Result, Result> sampleGoalDifferenceResults = new Dictionary<Result, Result>
                                       {
                                           {new Result { HomeGoals = 2, GuestGoals = 1 },
                                            new Result { HomeGoals = 3, GuestGoals = 2 }},

                                           {new Result { HomeGoals = 1, GuestGoals = 0 },
                                            new Result { HomeGoals = 5, GuestGoals = 4 }},                                           
                                            
                                           {new Result { HomeGoals = 1, GuestGoals = 3 },
                                            new Result { HomeGoals = 0, GuestGoals = 2 }},                                           
                                            
                                           {new Result { HomeGoals = 6, GuestGoals = 9 },
                                            new Result { HomeGoals = 3, GuestGoals = 6 }},

                                       };

        [Test]
        public void CalculateScore_should_return_proper_value_if_user_type_correct_goal_difference([ValueSource("sampleGoalDifferenceResults")] KeyValuePair<Result, Result> sampleResult)
        {
            var expectedScore = (int)Score.GoalDifference;
            this._bet.Game.Result = sampleResult.Key;
            this._bet.Result = sampleResult.Value;

            var resultScore = this._scorer.CalculateScore(this._bet);

            Assert.That(resultScore, Is.EqualTo(expectedScore));
        }

        private Dictionary<Result, Result> sampleWinTeamResults = new Dictionary<Result, Result>
                                       {
                                           {new Result { HomeGoals = 2, GuestGoals = 1 },
                                            new Result { HomeGoals = 3, GuestGoals = 0 }},

                                           {new Result { HomeGoals = 1, GuestGoals = 0 },
                                            new Result { HomeGoals = 5, GuestGoals = 0 }},                                           
                                            
                                           {new Result { HomeGoals = 1, GuestGoals = 3 },
                                            new Result { HomeGoals = 4, GuestGoals = 5 }},                                           
                                            
                                           {new Result { HomeGoals = 6, GuestGoals = 9 },
                                            new Result { HomeGoals = 3, GuestGoals = 4 }},

                                       };

        [Test]
        public void CalculateScore_should_return_proper_value_if_user_type_only_win_team([ValueSource("sampleWinTeamResults")] KeyValuePair<Result, Result> sampleResult)
        {
            var expectedScore = (int)Score.WinTeam;
            this._bet.Game.Result = sampleResult.Key;
            this._bet.Result = sampleResult.Value;

            var resultScore = this._scorer.CalculateScore(this._bet);

            Assert.That(resultScore, Is.EqualTo(expectedScore));
        }

        private Dictionary<Result, Result> sampleLooseResults = new Dictionary<Result, Result>
                                       {
                                           {new Result { HomeGoals = 2, GuestGoals = 1 },
                                            new Result { HomeGoals = 3, GuestGoals = 5 }},

                                           {new Result { HomeGoals = 1, GuestGoals = 0 },
                                            new Result { HomeGoals = 5, GuestGoals = 9 }},                                           
                                            
                                           {new Result { HomeGoals = 1, GuestGoals = 3 },
                                            new Result { HomeGoals = 4, GuestGoals = 0 }},                                           
                                            
                                           {new Result { HomeGoals = 6, GuestGoals = 1 },
                                            new Result { HomeGoals = 3, GuestGoals = 4 }},

                                       };

        [Test]
        public void CalculateScore_should_return_proper_value_if_user_type_bad([ValueSource("sampleLooseResults")] KeyValuePair<Result, Result> sampleResult)
        {
            var expectedScore = (int)Score.LoosOrNotBet;
            this._bet.Game.Result = sampleResult.Key;
            this._bet.Result = sampleResult.Value;

            var resultScore = this._scorer.CalculateScore(this._bet);

            Assert.That(resultScore, Is.EqualTo(expectedScore));
        }

        [Test]
        public void CalculatePositions_should_return_calculated_users_positions_and_totalscores()
        {
            var betsGroupByUser = new Dictionary<string, List<Bet>>
                                      {
                                          {
                                              "user1", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 2, Game = new Game()},
                                                      new Bet{Score = 3, Game = new Game()},
                                                      new Bet{Score = 1, Game = new Game()},
                                                      new Bet{Score = 0, Game = new Game()},
                                                  }
                                          },
                                          {
                                              "user2", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 3, Game = new Game()},
                                                      new Bet{Score = 3, Game = new Game()},
                                                      new Bet{Score = 1, Game = new Game()},
                                                      new Bet{Score = 0, Game = new Game()},
                                                  }
                                          },
                                          {
                                              "user3", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 2, Game = new Game()},
                                                      new Bet{Score = 3, Game = new Game()},
                                                  }
                                          },
                                          {
                                              "user4", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 3, Game = new Game()},
                                                      new Bet{Score = 3, Game = new Game()},
                                                      new Bet{Score = 2, Game = new Game()},
                                                  }
                                          }, 
                                          {
                                              "user5", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 2, Game = new Game()},
                                                      new Bet{Score = 3, Game = new Game()},
                                                      new Bet{Score = 1, Game = new Game()},
                                                      new Bet{Score = 1, Game = new Game()},
                                                      new Bet{Score = 0, Game = new Game()},
                                                  }
                                          },
                                          {
                                              "user6", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 2, Game = new Game()},
                                                      new Bet{Score = 3, Game = new Game()},
                                                      new Bet{Score = 0, Game = new Game()},
                                                  }
                                          },
                                      };

            var expectedResult = new List<Tuple<string, int, int>>
                                     {
                                         Tuple.Create("user4", 11, 1),
                                         Tuple.Create("user2", 10, 2),
                                         Tuple.Create("user5", 10, 3),
                                         Tuple.Create("user1", 9, 4),
                                         Tuple.Create("user3", 8, 5),
                                         Tuple.Create("user6", 8, 5),
                                     };

            var usersWithTotalScoresAndPosition = this._scorer.CalculatePositions(betsGroupByUser);

            Assert.That(usersWithTotalScoresAndPosition.Count, Is.EqualTo(expectedResult.Count));

            for (var i = 0; i < usersWithTotalScoresAndPosition.Count; i++)
            {
                Assert.That(usersWithTotalScoresAndPosition[i].Item1, Is.EqualTo(expectedResult[i].Item1));
                Assert.That(usersWithTotalScoresAndPosition[i].Item2, Is.EqualTo(expectedResult[i].Item2));
                Assert.That(usersWithTotalScoresAndPosition[i].Item3, Is.EqualTo(expectedResult[i].Item3));
            }
        }

        [Test]
        public void CalculatePositions_should_return_correct_users_positions_when_multiplier_is_active()
        {
            var betsGroupByUser = new Dictionary<string, List<Bet>>
                                      {
                                          {
                                              "user1", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 2, Game = new Game()},
                                                      new Bet{Score = 3, Game = new Game()},
                                                  }
                                          },
                                          {
                                              "user2", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 6, Game = new Game(){IsScoreMultiplier = true}}, 
                                                      new Bet{Score = 2, Game = new Game()},
                                                  }
                                          }, 
                                          {
                                              "user3", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 2, Game = new Game()},
                                                      new Bet{Score = 1, Game = new Game()},
                                                      new Bet{Score = 1, Game = new Game()},
                                                      new Bet{Score = 1, Game = new Game()},
                                                  }
                                          },  
                                          {
                                              "user4", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 4, Game = new Game(){IsScoreMultiplier = true}},
                                                      new Bet{Score = 0, Game = new Game(){IsScoreMultiplier = true}},
                                                     
                                                  }
                                          },
                                           {
                                              "user5", 
                                              new List<Bet>
                                                  {
                                                      new Bet{Score = 3, Game = new Game()}, 
                                                      new Bet{Score = 2, Game = new Game(){IsScoreMultiplier = true}},
                                                      new Bet{Score = 2, Game = new Game(){IsScoreMultiplier = true}},
                                                     
                                                  }
                                          },
                                      };

            var expectedResult = new List<Tuple<string, int, int>>
                                     {
                                         Tuple.Create("user1", 8, 1),
                                         Tuple.Create("user2", 8, 2),
                                         Tuple.Create("user3", 8, 2),
                                         Tuple.Create("user4", 7, 3),
                                         Tuple.Create("user5", 7, 4),
                                     };

            var usersWithTotalScoresAndPosition = this._scorer.CalculatePositions(betsGroupByUser);

            Assert.That(usersWithTotalScoresAndPosition.Count, Is.EqualTo(expectedResult.Count));

            for (var i = 0; i < usersWithTotalScoresAndPosition.Count; i++)
            {
                Assert.That(usersWithTotalScoresAndPosition[i].Item1, Is.EqualTo(expectedResult[i].Item1));
                Assert.That(usersWithTotalScoresAndPosition[i].Item2, Is.EqualTo(expectedResult[i].Item2));
                Assert.That(usersWithTotalScoresAndPosition[i].Item3, Is.EqualTo(expectedResult[i].Item3));
            }
        }

    }
}