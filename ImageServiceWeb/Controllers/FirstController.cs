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

        /// <summary>
        /// Gets the photo collection and shows the photos.
        /// </summary>
        /// <returns>The view of the photos.</returns>
        public ActionResult Photos()
        {
            return View(photosCollectionModel);
        }

        /// <summary>
        /// Gets a relative path to a photo (Relative to project directory) and shows the photo.
        /// </summary>
        /// <param name="photoRelPath">The relative path to the photo. (Relative to project directory).</param>
        /// <returns>The view of the photo.</returns>
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

        /// <summary>
        /// Gets a The thumbnail relative path of a photo (Relative to project directory),
        /// and returns the deletion confirmation view of the photo.
        /// </summary>
        /// <param name="ThumbPhotoRelPath"></param>
        /// <returns>The deletion confirmation view of the photo.</returns>
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

        /// <summary>
        /// Gets a The thumbnail relative path of a photo (Relative to project directory),
        /// deletes that photo from the photo collection, and returns to the photos view.
        /// </summary>
        /// <param name="thumbPhotoRelPath">The thumbnail relative path (Relative to project directory) of the photo
        /// to be deleted.</param>
        /// <returns>The photos view.</returns>
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