using System;

namespace CQRS2012.Gui.Models.Services
{
    public interface IBetStrategy
    {
        Result CalculateResult();
    }

    public class BetStrategyRandom: IBetStrategy
    {
        public Result CalculateResult()
        {
            var random = new Random();

            return new Result {HomeGoals = random.Next(4), GuestGoals = random.Next(4)};
        }
    }
}