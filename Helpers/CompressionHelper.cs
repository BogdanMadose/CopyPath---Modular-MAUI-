using System.IO.Compression;

namespace CopyPath___Modular_MAUI_.Helpers
{
    public static class CompressionHelper
    {
        public static void CompressFile(string filePath, string compressedFilePath)
        {
            using var originalFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var compressedFileStream = new FileStream(compressedFilePath, FileMode.Create);
            using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
            originalFileStream.CopyTo(compressionStream);
        }
    }
}
