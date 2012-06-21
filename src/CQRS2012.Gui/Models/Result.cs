using System;
using System.ComponentModel.DataAnnotations;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models
{
    [DbMap("Result")]
    public class Result
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "*")]
        public virtual int HomeGoals { get; set; }

        [Required(ErrorMessage = "*")]
        public virtual int GuestGoals { get; set; }
    }
}