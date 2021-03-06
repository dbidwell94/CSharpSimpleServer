namespace SimpleServer.Attributes
{
    /// <summary>
    /// Defines a PATCH path and how it should be handled
    /// </summary>
    public sealed class PatchMapping : AbstractMapping
    {
        public PatchMapping(string path) : base(path)
        {
        }
    }
}