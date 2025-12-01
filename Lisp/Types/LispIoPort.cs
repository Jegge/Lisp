using System.Text;

namespace Lisp.Types;

public class LispIoPort : LispValue, IDisposable
{
    private readonly string _filepath;
    private TextReader? _reader;
    private TextWriter? _writer;

    public FileAccess Access { get; }

    public LispIoPort (string filepath, FileAccess access)
    {
        Access = access;
        _filepath = filepath;
        switch (access)
        {
            case FileAccess.Read:
                _reader = new StreamReader(File.OpenRead(filepath), Encoding.UTF8);
                break;
            case FileAccess.Write:
                _writer = new StreamWriter(File.OpenWrite(filepath), Encoding.UTF8);
                break;
            case FileAccess.ReadWrite:
            default:
                throw new ArgumentOutOfRangeException(nameof(access));
        }
    }

    public string? Read () => _reader?.ReadToEnd();
    public bool Write (string value)
    {
        if (_writer is null)
            return false;
        _writer.Write(value);
        _writer.Flush();
        return true;
    }

    public bool Close ()
    {
        if (_reader != null && _reader != Console.In)
        {
            _reader.Close();
            _reader.Dispose();
            _reader = null;
            return true;
        }
        if (_writer != null && _writer != Console.Out)
        {
            _writer.Close();
            _writer.Dispose();
            _writer = null;
            return true;
        }
        return false;
    }

    public void Dispose ()
    {
        GC.SuppressFinalize(this);
        Close();
    }

    public override string Print (bool readable) => $"<io-port {LispString.Escape(_filepath)}>";
}