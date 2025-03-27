using CopyPath___Modular_MAUI_.Models;
using CopyPath___Modular_MAUI_.Views;

namespace CopyPath___Modular_MAUI_.Helpers
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    public static class FileHelper
    {
        private static bool _skipAllFiles;
        private static bool _overwriteAllFiles;

        public static Task<List<string>> FindConflictsAsync(List<string> filePaths)
            => Task.FromResult(filePaths.Where(File.Exists).ToList());

        public static async Task<ConflictResolution> HandleConflictsAsync(List<string> conflictingFiles)
        {
            if (conflictingFiles.Count == 0)
                return new ConflictResolution { ShouldProceed = true };

            var conflictDialog = new ConflictDialog(conflictingFiles);
            await Application.Current.Windows[0].Navigation.PushModalAsync(conflictDialog);
            var result = await conflictDialog.ShowAsync();

            return new ConflictResolution
            {
                ShouldProceed = result == "Overwrite" || result == "OverwriteAll",
                ApplyToAll = result.EndsWith("All"),
                Canceled = result == "Cancel"
            };
        }

        public static void ResetConflictResolution()
        {
            _skipAllFiles = false;
            _overwriteAllFiles = false;
        }
    }
}
