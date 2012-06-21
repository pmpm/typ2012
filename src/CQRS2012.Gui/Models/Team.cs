using System;
using System.ComponentModel.DataAnnotations;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models
{
    [DbMap("Team")]
    public class Team
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "*")]
        public virtual string Name { get; set; }

        public virtual string PathToFlag { get; set; }
    }
}