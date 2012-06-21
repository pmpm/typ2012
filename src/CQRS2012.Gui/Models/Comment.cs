using System;
using System.ComponentModel.DataAnnotations;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models
{
    [DbMap("Comment")]
    public class Comment
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "*")]
        public virtual string UserName { get; set; }

        [Required]
        public virtual DateTime TimeStamp { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public virtual string Content { get; set; }
    }
}