using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;


namespace LearningKitCore.Components.PageBuilder.ModalDialogs.ColorModalDialog
{
    public class ColorModalDialogController : Controller
    {
        public ActionResult Index()
        {
            var model = new ColorModalDialogViewModel
            {
                Colors = new List<string>() { "red", "blue", "white", "green", "black", "gray", "yellow" }
            };

            return View("~/Components/PageBuilder/ModalDialogs/ColorModalDialog/_ColorModalDialog.cshtml", model);
        }
    }
}