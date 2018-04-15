﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    /// <summary>
    /// Represents an image service modal interface.
    /// </summary>
    public interface IImageServiceModal
    {
        /// <summary>
        /// Addes A file to the system.
        /// </summary>
        /// <param name="path">The Path of the Image from the file.</param>
        /// <returns>Indication if the Addition Was Successful.</returns>
        string AddFile(string path, out bool result);
    }
}
