using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRS2012.Gui.Models.Services
{
    public interface IScorer
    {
        int CalculateScore(Bet bet);
        int CalculateTotalScore(List<Bet> userBets);
        List<Tuple<string, int, int>> CalculatePositions(Dictionary<string, List<Bet>> betGroupByUser);
    }

    public class Scorer : IScorer
    {
        public int CalculateScore(Bet bet)
        {
            var multiplier = (bet.Game.IsScoreMultiplier) ? 2 : 1;
            var realResult = bet.Game.Result;
            var betResult = bet.Result;
            var realGoalDifference = realResult.HomeGoals - realResult.GuestGoals;
            var betGoalDifference = betResult.HomeGoals - betResult.GuestGoals;

            if (realResult.HomeGoals == betResult.HomeGoals && realResult.GuestGoals == betResult.GuestGoals)
            {
                return (int)Score.ExactResult * multiplier;
            }

            if (realGoalDifference == betGoalDifference)
            {
                return (int)Score.GoalDifference * multiplier;
            }

            if ((realGoalDifference > 0 && betGoalDifference > 0) || (realGoalDifference < 0 && betGoalDifference < 0))
            {
                return (int)Score.WinTeam * multiplier;
            }

            return (int)Score.LoosOrNotBet;
        }

        public int CalculateTotalScore(List<Bet> userBets)
        {
            var totalScore = 0;
            userBets.ForEach(x => totalScore = +x.Score);

            return totalScore;
        }

        //Tuple<userName,totalscores,position>
        public List<Tuple<string, int, int>> CalculatePositions(Dictionary<string, List<Bet>> betGroupByUser)
        {
            var usersWithScoresData = betGroupByUser.Select(x => Tuple.Create(x.Key,
                                                                              x.Value.Sum(b => b.Score),
                                                                              x.Value.Where(b => b.Score == 3 || b.Score == 6).Count(),
                                                                              x.Value.Where(b => ( b.Score == 2 && !b.Game.IsScoreMultiplier) || b.Score == 4).Count()))
                                                    .OrderByDescending(x => x.Item2)
                                                    .ThenByDescending(x => x.Item3)
                                                    .ThenByDescending(x => x.Item4)
                                                    .ToList();

            var usersWithTotalScoresAndPosition = new List<Tuple<string, int, int>> { Tuple.Create(usersWithScoresData.ElementAt(0).Item1, usersWithScoresData.ElementAt(0).Item2, 1) };

            var position = 1;
            for (var i = 1; i < usersWithScoresData.Count(); i++)
            {
                position = position + 1;
                if (usersWithScoresData.ElementAt(i).Item2 == usersWithScoresData.ElementAt(i - 1).Item2 &&
                   usersWithScoresData.ElementAt(i).Item3 == usersWithScoresData.ElementAt(i - 1).Item3 &&
                   usersWithScoresData.ElementAt(i).Item4 == usersWithScoresData.ElementAt(i - 1).Item4)
                {
                    position = usersWithTotalScoresAndPosition[i - 1].Item3;
                }
                usersWithTotalScoresAndPosition.Add(Tuple.Create(usersWithScoresData.ElementAt(i).Item1, usersWithScoresData.ElementAt(i).Item2, position));
            }

            return usersWithTotalScoresAndPosition;
        }
    }
}