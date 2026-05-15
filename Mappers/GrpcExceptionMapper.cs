using Common.Exceptions;
using Grpc.Core;

namespace Common.Mappers;

public static class RpcExceptionMapper
{
    public static Exception Map(
        RpcException ex,
        string serviceName,
        string operation,
        CancellationToken cancellationToken
    )
    {
        if (ex.StatusCode == StatusCode.Cancelled && cancellationToken.IsCancellationRequested)
        {
            return new OperationCanceledException(cancellationToken);
        }

        var detail = string.IsNullOrWhiteSpace(ex.Status.Detail)
            ? $"{serviceName} {operation} request failed"
            : ex.Status.Detail.Trim();

        return ex.StatusCode switch
        {
            StatusCode.InvalidArgument => new BadRequestException(detail),
            StatusCode.NotFound => new NotFoundException(detail),
            StatusCode.ResourceExhausted => new TooManyRequestsException(detail),
            StatusCode.FailedPrecondition
                or StatusCode.AlreadyExists
                or StatusCode.Aborted => new ConflictException(detail),
            StatusCode.PermissionDenied => new ForbiddenException(detail),
            StatusCode.Unauthenticated => new UnauthorizedException(detail),
            StatusCode.DeadlineExceeded
                or StatusCode.Unavailable => new ExternalServiceException($"{serviceName} service unavailable: {detail}"),
            _ => new ExternalServiceException(
                $"{serviceName} service {operation} failed ({ex.StatusCode}): {detail}"
            )
        };
    }
}