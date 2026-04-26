namespace Common.Exceptions;

public class HttpException(string code, string message, int status) : Exception(message)
{
    public string Code { get; } = code;
    public int Status { get; } = status;
}

public class NotFoundException(string message) : HttpException("NOT_FOUND", message, 404);

public class ValidationException(string message) : HttpException("VALIDATION_ERROR", message, 400);

public class UnauthorizedException(string message) : HttpException("UNAUTHORIZED", message, 401);

public class ConflictException(string message) : HttpException("CONFLICT", message, 409);

public class ForbiddenException(string message) : HttpException("FORBIDDEN", message, 403);

public class InternalServerException(string message) : HttpException("INTERNAL_ERROR", message, 500);

public class ExternalServiceException(string message) : HttpException("EXTERNAL_ERROR", message, 502);
