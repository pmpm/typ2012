using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CQRS2012.Gui.Infrastructure;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Services;

namespace CQRS2012.Gui.Controllers
{
    [Authorize]
    public class ShoutboxController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ShoutboxController(ICommentService commentService, IDateTimeProvider dateTimeProvider)
        {
            this._commentService = commentService;
            this._dateTimeProvider = dateTimeProvider;
        }

        public ActionResult Index()
        {
            var newestComments = this._commentService.GetNewestComments();

            return View(newestComments);
        }

        [HttpPost]
        public ActionResult AddMessage(string message)
        {

            if (string.IsNullOrEmpty(message))
                return PartialView("Messages", this._commentService.GetNewestComments());

            var newComment = new Comment {Content = message, TimeStamp = this._dateTimeProvider.Now, UserName = User.Identity.Name};
         
            this._commentService.SaveComment(newComment);

            return PartialView("Messages", this._commentService.GetNewestComments());
        }



    }
}
