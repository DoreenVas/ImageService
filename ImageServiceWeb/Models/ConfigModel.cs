using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class ConfigModel
    {
        public ConfigModel()
        {
            OutputDir = "OutputDir";
            SourceName = "SourceName";
            LogName = "LogName";
            ThumbnailSize = "ThumbnailSize";
            Handlers = new List<string>();
            Handlers.Add("First handler");
            Handlers.Add("Second handler");

        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "OutputDir")]
        public string OutputDir { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "SourceName")]
        public string SourceName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "LogName")]
        public string LogName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ThumbnailSize")]
        public string ThumbnailSize { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Handlers")]
        public List<string> Handlers { get; set; }

    }
}