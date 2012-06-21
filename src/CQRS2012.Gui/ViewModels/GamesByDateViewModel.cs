using System;
using System.Collections.Generic;
using CQRS2012.Gui.Models;

namespace CQRS2012.Gui.ViewModels
{
    public class GamesByDateViewModel
    {
        public List<Game> Games { get; set; }
        public DateTime GamesDate { get; set; }
    }
}