using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Services;

namespace CQRS2012.Gui.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRankingService _rankingService;

        public HomeController(IRankingService rankingService)
        {
            this._rankingService = rankingService;
        }

        public ActionResult Index()
        {
            return View(this._rankingService.GetMainRanking());
        }
    }
}
