using SimpleServer.Networking.Data;

namespace SimpleServer.Exceptions
{
    public interface IServerExceptionHandler
    {
        ResponseEntity HandleEndpointNotValidException(ServerEndpointNotValidException exception);
        ResponseEntity HandleServerRequestMethodNotSupportedException(ServerRequestMethodNotSupportedException exception);
        ResponseEntity HandleAbstractServerException(AbstractServerException exception);
        ResponseEntity HandleInternalServerErrorException(InternalServerErrorException exception);
    }
}