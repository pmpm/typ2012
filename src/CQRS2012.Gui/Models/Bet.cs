using System;
using System.ComponentModel.DataAnnotations;
using CQRS2012.Gui.Models.Database;

namespace CQRS2012.Gui.Models
{
    [DbMap("Bet")]
    public class Bet
    {
        public virtual Guid Id { get; set; }
        public virtual string UserName { get; set; }

        [Required]
        public virtual Game Game { get; set; }

        public virtual Result Result { get; set; } 
        public virtual int Score { get; set; }
    }

    public enum Score
    {
        LoosOrNotBet = 0,
        WinTeam = 1,
        GoalDifference = 2,
        ExactResult = 3,
    }
}