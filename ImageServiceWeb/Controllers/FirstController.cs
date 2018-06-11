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
        static PhotosCollectionModel photosCollectionModel = new PhotosCollectionModel();
        static MainModel mainModel = new MainModel();
        private static string handlerDelete;

        public FirstController()
        {
            if (mainModel.Connected == true)
            {
                while (configModel.OutputDir == null)
                {
                    Thread.Sleep(1000);
                }
                string outputDirPath = configModel.OutputDir;
                photosCollectionModel.GetPhotosCollection(outputDirPath);
            }
        }
        // GET: First
        public ActionResult Index()
        {
            mainModel.PhotoNumber = photosCollectionModel.Photos.Count;
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

        public ActionResult PhotoPresenter(string photoRelPath)
        {
            foreach (PhotosModel photo in photosCollectionModel.Photos)
            {
                if (photo.RelativePath == photoRelPath)
                {
                    return View(photo);
                }
            }
            return null;
        }

        public ActionResult DeletePhoto(string ThumbPhotoRelPath)
        {
            foreach (PhotosModel photo in photosCollectionModel.Photos)
            {
                if (photo.RelativeThumbnailPath == ThumbPhotoRelPath)
                {
                    return View(photo);
                }
            }
            return null;
        }

        public ActionResult PhotoDeletionConfirmed(string thumbPhotoRelPath)
        {
            foreach (PhotosModel photo in photosCollectionModel.Photos)
            {
                if (photo.RelativeThumbnailPath == thumbPhotoRelPath)
                {
                    System.IO.File.Delete(photo.FullPath);
                    System.IO.File.Delete(photo.ThumbFullPath);
                    return RedirectToAction("Photos");
                }
            }
            return null;
        }
    }

}