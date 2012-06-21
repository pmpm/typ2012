using System;

namespace CQRS2012.Gui.Models.Services
{
    public interface IStrategyFactory
    {
        IBetStrategy Create(string strategyName);
    }

    public class StrategyFactory : IStrategyFactory
    {

        public IBetStrategy Create(string strategyName)
        {
            if (strategyName == null)
            {
                throw new ArgumentNullException("strategyName");
            }

            if (strategyName == Strategy.Randomer.ToString())
            {
                return new BetStrategyRandom();
            }

            throw new NotSupportedException(string.Format("Strategia {0} jest nie wspierana", strategyName));
        }
    }

    public enum Strategy
    {
        Randomer
    }
}