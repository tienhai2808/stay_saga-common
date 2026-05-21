namespace Common.Events;

public record PaymentSuccessEvent
{
    public long BookingId { get; init; }
    public string KeycloakId { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}