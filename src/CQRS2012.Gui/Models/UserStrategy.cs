using System;
using System.ComponentModel.DataAnnotations;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models
{
    [DbMap("UserStrategy")]
    public class UserStrategy
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "*")]
        public virtual string UserName { get; set; }
        [Required(ErrorMessage = "*")]
        public virtual string StrategyName { get; set; }
    }
}