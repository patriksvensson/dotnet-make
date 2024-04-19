using Spectre.Console.Rendering;

namespace Make;

public class MakeException : Exception
{
    /// <summary>
    /// Gets the pretty formatted error.
    /// This might be an renderable exception or something else.
    /// </summary>
    public IRenderable? Pretty { get; }

    public MakeException(string message, IRenderable? pretty = null)
        : base(message)
    {
        Pretty = pretty;
    }

    public MakeException(string message, Exception ex, IRenderable? pretty = null)
        : base(message, ex)
    {
        Pretty = pretty;
    }
}