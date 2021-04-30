using System.IO;

public void DeleteDirectory(string path)
{
    if( Directory.Exists(path))
        Directory.Delete(path, recursive: true);
}
