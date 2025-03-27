using CopyPath___Modular_MAUI_.Helpers;
using CopyPath___Modular_MAUI_.Models;

namespace CopyPath___Modular_MAUI_.Services
{
#pragma warning disable CS8604 // Possible null reference argument.
    public class FileTransferService
    {
        public static async Task TransferFilesAsync(FileTransferOptions? options,
                                           CancellationToken cancellationToken,
                                           IProgress<(double Percentage, int Processed, int Total)>? progress = null)
        {
            FileHelper.ResetConflictResolution();

            try
            {
                // 1. Get all files with full paths
                var allFiles = Directory.EnumerateFiles(options?.Source, "*", SearchOption.AllDirectories).ToList();
                int totalFiles = allFiles.Count;
                int processedFiles = 0;

                // 2. Create destination root directory
                Directory.CreateDirectory(options.Destination);

                // 3. Find all potential conflicts first
                var potentialConflicts = allFiles
                    .Select(sourceFile =>
                    {
                        var relativePath = Path.GetRelativePath(options.Source, sourceFile);
                        return Path.Combine(options.Destination, relativePath);
                    })
                    .Where(File.Exists)
                    .ToList();

                // 4. Resolve conflicts in bulk
                var resolution = await FileHelper.HandleConflictsAsync(potentialConflicts);

                if (resolution.Canceled) throw new OperationCanceledException();

                // 5. Process all files
                foreach (var sourceFile in allFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var relativePath = Path.GetRelativePath(options.Source, sourceFile);
                    var destFile = Path.Combine(options.Destination, relativePath);

                    // Create target directory structure
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                    // Handle file transfer based on resolution
                    if (resolution.ApplyToAll || !potentialConflicts.Contains(destFile))
                    {
                        if (resolution.ShouldProceed || !potentialConflicts.Contains(destFile))
                            await Task.Run(() => File.Copy(sourceFile, destFile, true), cancellationToken);
                        else throw new OperationCanceledException("User skipped file transfer.");
                    }
                    else if (potentialConflicts.Contains(destFile) && !resolution.ApplyToAll)
                    {
                        // Individual file decision needed
                        var fileResolution = await FileHelper.HandleConflictsAsync([destFile]);
                        if (fileResolution.ShouldProceed)
                            await Task.Run(() => File.Copy(sourceFile, destFile, true), cancellationToken);
                    }

                    // Update progress
                    processedFiles++;
                    progress?.Report((
                                (double)processedFiles / totalFiles,
                                processedFiles,
                                totalFiles
                            ));
                }
            }
            catch (OperationCanceledException)
            {
                LoggingHelper.LogInfo("Transfer canceled by user");
                throw;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Transfer failed", ex);
                throw;
            }
        }

        private async Task CopyDirectoryAsync(string sourceDir, string destDir, CancellationToken cancellationToken)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

            Directory.CreateDirectory(destDir);

            // Process files first
            await ProcessFilesAsync(dir, destDir, cancellationToken);

            // Process subdirectories recursively
            foreach (var subdir in dir.GetDirectories())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var newDestDir = Path.Combine(destDir, subdir.Name);
                await CopyDirectoryAsync(subdir.FullName, newDestDir, cancellationToken);
            }
        }

        private async Task ProcessFilesAsync(DirectoryInfo dir, string destDir, CancellationToken cancellationToken)
        {
            var files = dir.GetFiles();
            var potentialConflicts = files.Select(f => Path.Combine(destDir, f.Name)).ToList();
            var conflicts = await FileHelper.FindConflictsAsync(potentialConflicts);

            if (conflicts.Count == 0)
            {
                // Fast path: no conflicts, copy all
                await ParallelForEachAsync(files, async file =>
                {
                    var destPath = Path.Combine(destDir, file.Name);
                    await CopyFileAsync(file, destPath, cancellationToken);
                });
                return;
            }

            var resolution = await FileHelper.HandleConflictsAsync(conflicts);

            if (resolution.Canceled) throw new OperationCanceledException("User canceled file transfer.");

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var destPath = Path.Combine(destDir, file.Name);

                if (ShouldCopyFile(resolution, destPath, conflicts)) await CopyFileAsync(file, destPath, cancellationToken);
            }
        }

        private static bool ShouldCopyFile(ConflictResolution resolution, string destPath, List<string> conflicts)
        {
            if (resolution.ApplyToAll) return resolution.ShouldProceed;
            if (!conflicts.Contains(destPath)) return true;
            return resolution.ShouldProceed;
        }

        private static async Task CopyFileAsync(FileInfo file, string destPath, CancellationToken ct)
            => await Task.Run(() => file.CopyTo(destPath, overwrite: true), ct);

        private static async Task ParallelForEachAsync<T>(IEnumerable<T> items, Func<T, Task> action)
        {
            var tasks = items.Select(action);
            await Task.WhenAll(tasks);
        }
    }
}