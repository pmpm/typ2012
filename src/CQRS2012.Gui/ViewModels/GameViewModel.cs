using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CQRS2012.Gui.Models;

namespace CQRS2012.Gui.ViewModels
{
    public class GameViewModel
    {
        public List<Game> Games{get; set;}
    }
}