using System.Collections.Generic;
using CQRS2012.Gui.Models.Repositories;

namespace CQRS2012.Gui.Models.Services
{
    public interface ICommentService
    {
        IList<Comment> GetNewestComments();
        void SaveComment(Comment comment);
    }

    public class CommentService : ICommentService
    {
        private const int MaxCommentsCount = 100;
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            this._commentRepository = commentRepository;
        }

        public IList<Comment> GetNewestComments()
        {
            return this._commentRepository.GetNewestComments(MaxCommentsCount);
        }

        public void SaveComment(Comment comment)
        {
            if (this._commentRepository.GetCommentsCount() > MaxCommentsCount)
            {
                this._commentRepository.DeleteOldestComment();
            }

            this._commentRepository.Save(comment);
        }
    }
}