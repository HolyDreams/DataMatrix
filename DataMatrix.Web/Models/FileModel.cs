namespace DataMatrix.Web.Models
{
    public class FileModel : IDisposable
    {
        public FileModel(string dir, byte[] content)
        {
            _path = Path.Combine(dir, FileName);
            using (var fs = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.None, content.Length))
                fs.Write(content, 0, content.Length);
        }

        public FileModel(string dir, Stream stream)
        {
            _path = Path.Combine(dir, FileName);
            using (var fs = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.None))
                stream.CopyTo(fs);
        }

        private readonly string _path;

        public string FileName { get; init; } = Guid.NewGuid() + ".png";

        public byte[] Content { get => File.ReadAllBytes(_path); }

        public Stream Stream { get => new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.None); }

        public void Dispose()
        {
            if (File.Exists(_path))
                File.Delete(_path);
        }
    }
}
