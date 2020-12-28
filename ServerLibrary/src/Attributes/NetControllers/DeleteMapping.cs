namespace SimpleServer.Attributes
{
    /// <summary>
    /// Defines a DELETE path and how it should be handled
    /// </summary>
    public sealed class DeleteMapping : AbstractMapping
    {
        public DeleteMapping(string path) : base(path)
        {

        }
    }
}