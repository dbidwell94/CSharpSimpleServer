namespace SimpleServer.Attributes
{
    /// <summary>
    /// Defines a PUT path and how it should be handled
    /// </summary>
    public sealed class PutMapping : AbstractMapping
    {
        public PutMapping(string path) : base(path)
        {

        }
    }
}