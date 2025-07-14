namespace Bonyan.VirtualFileSystem;

public class BonVirtualFileSystemOptions
{
    public VirtualFileSetList FileSets { get; }

    public BonVirtualFileSystemOptions()
    {
        FileSets = new VirtualFileSetList();
    }
}
