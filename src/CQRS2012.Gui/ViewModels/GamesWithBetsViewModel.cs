using System;
using System.Collections.Generic;
using CQRS2012.Gui.Models;

namespace CQRS2012.Gui.ViewModels
{
    public class GamesWithBetsViewModel
    {
        public Dictionary<Game,Bet> GamesWithBets { get; set; }
        public DateTime GamesDate { get; set; }
    }
}