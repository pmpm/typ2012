using System.Collections.Generic;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models.Repositories
{
    public interface IStrategyRepository
    {
        void BindUserWithStrategy(UserStrategy userStrategy);
        string GetStrategyForUser(string userName);
        IList<UserStrategy> GetAllUserStrategy();
    }

    public class StrategyRepository : IStrategyRepository
    {
        public IList<UserStrategy> GetAllUserStrategy()
        {
            return (List<UserStrategy>)FnHibernateHelper.InTransaction(session => session.QueryOver<UserStrategy>().List());
        }

        public string GetStrategyForUser(string userName)
        {
            var userStrategy = (UserStrategy)FnHibernateHelper.InTransaction(session => session.QueryOver<UserStrategy>()
                                                                                               .Where(x => x.UserName == userName)
                                                                                               .SingleOrDefault());

            return userStrategy.StrategyName;
        }

        public void BindUserWithStrategy(UserStrategy userStrategy)
        {
            FnHibernateHelper.InTransaction(session => session.Save(userStrategy));
        }
    }
}