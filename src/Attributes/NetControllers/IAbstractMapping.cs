namespace SimpleServer.Attributes
{
    public interface IAbstractMapping
    {
        string Path { get; set; }
        string Produces { get; set; }
        string Accepts { get; set; }
    }
}