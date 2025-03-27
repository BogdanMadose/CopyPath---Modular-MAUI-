namespace CopyPath___Modular_MAUI_.Models
{
    public class ConflictResolution
    {
        public bool ShouldProceed { get; set; }  // true=overwrite, false=skip
        public bool ApplyToAll { get; set; }     // whether to apply to all files
        public bool Canceled { get; set; }       // whether operation was canceled
    }
}
