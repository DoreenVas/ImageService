using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class PhotosModel
    {

        private string name;
        private string relativePath;
        //private string fullPath;
        private string thumbRelativePath;
        //private string thumbFullPath;
        private string year;
        private string month;

        public PhotosModel(string photoName, string photoRelativePath, string thumbnailRelativePath,
            string photoYear, string photoMonth)
        {
            name = photoName;
            relativePath = photoRelativePath;
            thumbRelativePath = thumbnailRelativePath;
            //fullPath = photofullPath; // todo: maybe remove abs paths
            //thumbFullPath = thumbnailFullPath;
            year = photoYear;
            month = photoMonth;
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get => name; set => name = value; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "RelativePath")]
        public string RelativePath { get => relativePath; set => relativePath = value; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "RelativeThumbnailPath")]
        public string RelativeThumbnailPath { get => thumbRelativePath; set => thumbRelativePath = value; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Year")]
        public string Year { get => year; set => year = value; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Month")]
        public string Month { get => month; set => month = value; }
    }
}