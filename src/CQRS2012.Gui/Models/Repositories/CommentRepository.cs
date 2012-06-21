using System.Collections.Generic;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models.Repositories
{
    public interface ICommentRepository
    {
        IList<Comment> GetNewestComments(int commentsCount);
        void Save(Comment comment);
        int GetCommentsCount();
        void DeleteOldestComment();
    }

    public class CommentRepository : ICommentRepository
    {
        public IList<Comment> GetNewestComments(int commentsCount)
        {
            return (List<Comment>)FnHibernateHelper.InTransaction(session => session.QueryOver<Comment>()
                                                                                    .OrderBy(c => c.TimeStamp).Desc
                                                                                    .Take(commentsCount).List());
        }

        public void Save(Comment comment)
        {
            FnHibernateHelper.InTransaction(session => session.Save(comment));
        }

        public int GetCommentsCount()
        {
            return (int)FnHibernateHelper.InTransaction(session => session.QueryOver<Comment>().RowCount());
        }

        public void DeleteOldestComment()
        {
            FnHibernateHelper.InTransaction(session =>
                        {
                            var oldestComment = session.QueryOver<Comment>().OrderBy(c => c.TimeStamp).Asc.Take(1).SingleOrDefault();
                            session.Delete(oldestComment);
                        }
            );
    
        }
    }
}