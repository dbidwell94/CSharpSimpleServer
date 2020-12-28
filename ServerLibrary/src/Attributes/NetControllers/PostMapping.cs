namespace SimpleServer.Attributes
{
    /// <summary>
    /// Defines a POST path and how it should be handled
    /// </summary>
    public sealed class PostMapping : AbstractMapping
    {
        public PostMapping(string path) : base(path)
        {

        }
    }
}