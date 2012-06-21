using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models
{
    [DbMap("Games")]
    public class Game
    {
        public virtual Guid Id{ get; set;}
     
        public virtual Team HomeTeam { get; set; }
        public virtual Team GuestTeam { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public virtual DateTime GameStartDate { get; set; }

        public virtual bool IsFinished { get; set; }
        public virtual Result Result { get; set; }
        public virtual bool IsScoreMultiplier { get; set; }
    }
}