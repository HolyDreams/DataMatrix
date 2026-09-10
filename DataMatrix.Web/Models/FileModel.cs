namespace DataMatrix.Web.Models
{
    public class FileModel : IDisposable
    {
        public FileModel(string dir, int id, byte[] content, bool overrideFile = false)
        {
            FileName = id + ".png";
            _path = Path.Combine(dir, FileName);
            if (overrideFile)
            {
                var fileInfo = new FileInfo(_path);
                if (fileInfo.Exists && fileInfo.Length == content.Length)
                    return;                 
            }
            if (File.Exists(_path))
                File.Delete(_path);

            using (var fs = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.None, content.Length))
                fs.Write(content, 0, content.Length);
        }

        public FileModel(string dir, int id, Stream stream, bool overrideFile = false)
        {
            FileName = id + ".png";
            _path = Path.Combine(dir, FileName);
            if (overrideFile)
            {
                var fileInfo = new FileInfo(_path);
                if (fileInfo.Exists && fileInfo.Length == stream.Length)
                    return;
            }
            if (File.Exists(_path))
                File.Delete(_path);

            using (var fs = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.None))
                stream.CopyTo(fs);
        }

        public FileModel(string path)
        {
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                throw new FileNotFoundException(path);
            _path = path;
            FileName = fileInfo.Name;
        }

        private readonly string _path;

        public string FileName { get; init; }

        public byte[] Content { get => File.ReadAllBytes(_path); }

        public Stream Stream { get => new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.None); }

        public void Dispose()
        {
            if (File.Exists(_path))
                File.Delete(_path);
        }
    }
}
