namespace SimpleServer.Attributes
{
    /// <summary>
    /// Defines a GET path and how it should be handled
    /// </summary>
    public sealed class GetMapping : AbstractMapping
    {
        public GetMapping(string path) : base(path)
        {
        }
    }
}