// <copyright file="Booking.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;

namespace Beddin.Domain.Aggregates.Properties
{
    /// <summary>
    /// Represents a booking for a property, including details about the viewer, owner, scheduled time, and status.
    /// </summary>
    public sealed class Booking : AggregateRoot<BookingId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Booking"/> class.
        /// </summary>
        public Booking()
        {
        }

        /// <summary>
        /// Gets the identifier of the property associated with this booking.
        /// </summary>
        public PropertyId PropertyId { get; private set; } = null!;

        /// <summary>
        /// Gets the identifier of the viewer associated with this booking.
        /// </summary>
        public UserId ViewerId { get; private set; } = null!;

        /// <summary>
        /// Gets the identifier of the owner associated with this booking.
        /// </summary>
        public UserId OwnerId { get; private set; } = null!;

        /// <summary>
        /// Gets the date and time when the booking is scheduled to occur.
        /// </summary>
        public DateTime ScheduledAt { get; private set; }

        /// <summary>
        /// Gets the status of the booking.
        /// </summary>
        public BookingStatus Status { get; private set; }

        /// <summary>
        /// Gets the date and time when the booking was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the booking was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Gets the property associated with this booking.
        /// </summary>
        public Property Property { get; private set; } = null!;

        /// <summary>
        /// Schedules a new booking for a property with the specified viewer, owner, and scheduled time.
        /// </summary>
        /// <param name="propertyId">The identifier of the property to be booked.</param>
        /// <param name="viewerId">The identifier of the user viewing the property.</param>
        /// <param name="ownerId">The identifier of the owner of the property.</param>
        /// <param name="scheduledAt">The date and time when the booking is scheduled to occur.</param>
        /// <returns>A new <see cref="Booking"/> instance with status set to pending.</returns>
        /// <exception cref="ArgumentException">Thrown when the scheduled time is not in the future.</exception>
        public static Booking Schedule(
            PropertyId propertyId,
            UserId viewerId,
            UserId ownerId,
            DateTime scheduledAt)
        {
            if (scheduledAt <= DateTime.UtcNow)
            {
                throw new ArgumentException("Scheduled time must be in the future");
            }

            return new Booking
            {
                Id = BookingId.New(),
                PropertyId = propertyId,
                ViewerId = viewerId,
                OwnerId = ownerId,
                ScheduledAt = scheduledAt,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Confirms the booking if it is currently pending.
        /// </summary>
        /// <returns>
        /// A <see cref="Result"/> indicating success if the booking was confirmed; otherwise, a failure result with an error message.
        /// </returns>
        public Result Confirm()
        {
            if (this.Status != BookingStatus.Pending)
            {
                return Result.Failure("Only pending bookings can be confirmed");
            }

            this.Status = BookingStatus.Confirmed;
            this.UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        /// <summary>
        /// Cancels the booking if the actor is the viewer or owner and the booking is not already cancelled or completed.
        /// </summary>
        /// <param name="actor">The user attempting to cancel the booking.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating success if the booking was cancelled; otherwise, a failure result with an error message.
        /// </returns>
        public Result Cancel(UserId actor)
        {
            if (this.Status == BookingStatus.Cancelled || this.Status == BookingStatus.Completed)
            {
                return Result.Failure("Cannot cancel completed or already cancelled booking");
            }

            if (actor != this.ViewerId && actor != this.OwnerId)
            {
                return Result.Failure("Only the viewer or owner can cancel this booking");
            }

            this.Status = BookingStatus.Cancelled;
            this.UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        /// <summary>
        /// Marks the booking as completed if it is currently confirmed.
        /// </summary>
        /// <returns>
        /// A <see cref="Result"/> indicating success if the booking was completed; otherwise, a failure result with an error message.
        /// </returns>
        public Result Complete()
        {
            if (this.Status != BookingStatus.Confirmed)
            {
                return Result.Failure("Only confirmed bookings can be completed");
            }

            this.Status = BookingStatus.Completed;
            this.UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        /// <summary>
        /// Reschedules the booking to a new time if the actor is the viewer and the booking is not cancelled or completed.
        /// </summary>
        /// <param name="newTime">The new date and time for the booking.</param>
        /// <param name="actor">The user attempting to reschedule the booking.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating success if the booking was rescheduled; otherwise, a failure result with an error message.
        /// </returns>
        public Result Reschedule(DateTime newTime, UserId actor)
        {
            if (actor != this.ViewerId)
            {
                return Result.Failure("Only the viewer can reschedule the booking");
            }

            if (this.Status == BookingStatus.Cancelled || this.Status == BookingStatus.Completed)
            {
                return Result.Failure("Cannot reschedule cancelled or completed booking");
            }

            if (newTime <= DateTime.UtcNow)
            {
                return Result.Failure("New schedule must be in the future");
            }

            this.ScheduledAt = newTime;
            this.UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }
    }
}
