using CopyPath___Modular_MAUI_.Views;

namespace CopyPath___Modular_MAUI_.Helpers
{
    public static class FileHelper
    {
        private static bool _skipAllFiles;
        private static bool _overwriteAllFiles;

        public static async Task<List<string>> FindConflictsAsync(List<string> filePaths) => filePaths.Where(File.Exists).ToList();

        public static async Task<ConflictResolution> HandleConflictsAsync(List<string> conflictingFiles)
        {
            if (!conflictingFiles.Any())
                return new ConflictResolution { ShouldProceed = true };

            var conflictDialog = new ConflictDialog(conflictingFiles);
            await Application.Current.MainPage.Navigation.PushModalAsync(conflictDialog);
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

        public class ConflictResolution
        {
            public bool ShouldProceed { get; set; }  // true=overwrite, false=skip
            public bool ApplyToAll { get; set; }     // whether to apply to all files
            public bool Canceled { get; set; }       // whether operation was canceled
        }
    }
}
