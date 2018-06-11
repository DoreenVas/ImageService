using ImageServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWeb.Controllers
{
    public class FirstController : Controller
    {
        static ConfigModel configModel = new ConfigModel();
        static LogsModel logsModel = new LogsModel();
        
        static PhotosCollectionModel photosCollectionModel = new PhotosCollectionModel("C:\\Users\\Doreen\\source\\repos\\ImageService\\ImageServiceWeb\\to");
        static MainModel mainModel = new MainModel();
        private static string handlerDelete;

        // GET: First
        public ActionResult Index()
        {
            return View(mainModel);
        }

        [HttpGet]
        public ActionResult Config()
        {
            return View(configModel);
        }

        [HttpGet]
        public ActionResult DeleteHandler()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logs()
        {
            return View(logsModel);
        }

        public ActionResult SaveHandler(string handlerToDelete)
        {
            handlerDelete = handlerToDelete;
            return RedirectToAction("DeleteHandler");
        }

        public ActionResult OKClicked()
        {
            configModel.DeleteHandler(handlerDelete);
            while (configModel.Handlers.Contains(handlerDelete))
            {
                Thread.Sleep(1000);
            }
            return RedirectToAction("Config");
        }


        public ActionResult Photos()
        {
            return View(photosCollectionModel);
        }
    }
}