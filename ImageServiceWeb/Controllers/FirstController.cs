using ImageServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWeb.Controllers
{
    public class FirstController : Controller
    {
        static ConfigModel configModel = new ConfigModel();
        static PhotosModel photosModel = new PhotosModel();
        // GET: First
        public ActionResult Index()
        {
            return View();
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
        public ActionResult Image()
        {
            return View(photosModel);
        }
    }
}