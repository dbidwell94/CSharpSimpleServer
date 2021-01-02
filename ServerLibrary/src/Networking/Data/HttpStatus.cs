namespace SimpleServer.Networking.Data
{
    public enum HttpStatus
    {
        CONTINUE = 100,
        SWITCH_PROTOCOL = 101,
        OK = 200,
        CREATED = 201,
        ACCEPTED = 202,
        NO_CONTENT = 204,
        PARTIAL_CONTENT = 206,
        MOVED_PERMANENTLY = 301,
        FOUND = 302,
        TEMPORARY_REDIRECT = 307,
        PERMANENT_REDIRECT = 308,
        BAD_REQUEST = 400,
        UNAUTHORIZED = 401,
        NOT_FOUND = 404,
        METHOD_NOT_ALLOWED = 405,
        NOT_ACCEPTABLE = 406,
        REQUEST_TIMEOUT = 408,
        LENGTH_REQUIRED = 411,
        URI_TOO_LONG = 414,
        UNSUPPORTED_MEDIA_TYPE = 415,
        RANGE_NOT_SATISFIABLE = 416,
        IM_A_TEAPOT = 418,
        TOO_MANY_REQUESTS = 429,
        REQUEST_HEADER_FIELDS_TOO_LONG = 431,
        INTERNAL_SERVER_ERROR = 500,
        NOT_IMPLEMENTED = 501,
        BAD_GATEWAY = 502,
        SERVICE_UNAVAILABLE = 503,
        GATEWAY_TIMEOUT = 504,
        HTTP_VERSION_NOT_SUPPORTED = 505,
        LOOP_DETECTED = 508
    }
}