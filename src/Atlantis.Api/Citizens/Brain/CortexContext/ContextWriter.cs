using System.Text;

namespace Atlantis.Api.Citizens.Brain.CortexContext;

public sealed class ContextWriter
{
    private readonly StringBuilder _builder =
        new();

    public void Write(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _builder.Append(value);
    }

    public void WriteLine()
    {
        _builder.AppendLine();
    }

    public void WriteLine(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _builder.AppendLine(value);
    }

    public void WriteRaw(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _builder.Append(value);
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}