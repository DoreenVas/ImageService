using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class PhotosCollectionModel
    {
        const string backSlashToken = "/";
        const string thumbnailsToken = "/Thumbnails";

        [Display(Name = "PhotosCollection")]
        public List<PhotosModel> Photos { get; set; } = new List<PhotosModel>();

        public void GetPhotosCollection(string outputDirPath)
        {
            if (outputDirPath == null)
            {
                return;
            }

            Photos.Clear();
            DirectoryInfo directory = new DirectoryInfo(outputDirPath);
            List<DirectoryInfo> directoriesByYears = directory.GetDirectories().ToList();
            foreach (DirectoryInfo yearDirectory in directoriesByYears)
            {
                List<DirectoryInfo> directoriesByMonths = yearDirectory.GetDirectories().ToList();
                if (yearDirectory.Name != "Thumbnails")
                {
                    
                    foreach (DirectoryInfo monthDirectory in directoriesByMonths)
                    {
                        List<FileInfo> photosCol = monthDirectory.GetFiles().ToList();
                        foreach (FileInfo photo in photosCol)
                        {
                            string yearStr = yearDirectory.Name;
                            string monthStr = monthDirectory.Name;
                            string relPath = backSlashToken + yearDirectory.Parent.Name + backSlashToken + yearStr + backSlashToken +
                                monthStr + backSlashToken + photo.Name;
                            string thumbRelPath = backSlashToken + yearDirectory.Parent.Name + thumbnailsToken + backSlashToken + yearStr + backSlashToken +
                                monthStr + backSlashToken + photo.Name;
                            string fullPath = outputDirPath + backSlashToken + yearStr + backSlashToken + monthStr +
                                backSlashToken + photo.Name;
                            string thumbFullPath = outputDirPath + thumbnailsToken + backSlashToken + yearStr + backSlashToken +
                                monthStr + backSlashToken + photo.Name;

                            Photos.Add(new PhotosModel(photo.Name, relPath, thumbRelPath, fullPath, thumbFullPath, yearStr, monthStr));
                        }
                    }
                }

            }
        }
    }
}