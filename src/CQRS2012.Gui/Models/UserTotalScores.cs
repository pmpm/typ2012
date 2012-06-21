using System;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models
{
    [DbMap("UserTotalScores")]
    public class UserTotalScores
    {
        public virtual Guid Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual int TotalScore { get; set; }
        public virtual int Position { get; set; }
    }
}