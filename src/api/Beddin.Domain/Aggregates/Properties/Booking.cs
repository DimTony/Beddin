using Beddin.Domain.Common;

namespace Beddin.Domain.Aggregates.Properties
{
    public sealed class Booking : AggregateRoot<BookingId>
    {
        public PropertyId PropertyId { get; private set; } = null!;
        public UserId ViewerId { get; private set; } = null!;
        public UserId OwnerId { get; private set; } = null!;
        public DateTime ScheduledAt { get; private set; }
        public BookingStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Property Property { get; private set; } = null!;
        public Booking() { }
        public static Booking Schedule(
            PropertyId propertyId,
            UserId viewerId,
            UserId ownerId,
            DateTime scheduledAt)
        {
            if (scheduledAt <= DateTime.UtcNow)
                throw new ArgumentException("Scheduled time must be in the future");

            return new Booking
            {
                Id = BookingId.New(),
                PropertyId = propertyId,
                ViewerId = viewerId,
                OwnerId = ownerId,
                ScheduledAt = scheduledAt,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public Result Confirm()
        {
            if (Status != BookingStatus.Pending)
                return Result.Failure("Only pending bookings can be confirmed");

            Status = BookingStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();

        }

        public Result Cancel(UserId actor)
        {
            if (Status == BookingStatus.Cancelled || Status == BookingStatus.Completed)
                return Result.Failure("Cannot cancel completed or already cancelled booking");

            if (actor != ViewerId && actor != OwnerId)
                return Result.Failure("Only the viewer or owner can cancel this booking");

            Status = BookingStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Complete()
        {
            if (Status != BookingStatus.Confirmed)
                return Result.Failure("Only confirmed bookings can be completed");

            Status = BookingStatus.Completed;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Reschedule(DateTime newTime, UserId actor)
        {
            if (actor != ViewerId)
                return Result.Failure("Only the viewer can reschedule the booking");

            if (Status == BookingStatus.Cancelled || Status == BookingStatus.Completed)
                return Result.Failure("Cannot reschedule cancelled or completed booking");

            if (newTime <= DateTime.UtcNow)
                return Result.Failure("New schedule must be in the future");

            ScheduledAt = newTime;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

    }
}
