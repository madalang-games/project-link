namespace ProjectLink.Domain.Exceptions;

public class DailyChallengeAlreadyCompletedException : DomainException
{
    public DailyChallengeAlreadyCompletedException()
        : base("DAILY_CHALLENGE_ALREADY_COMPLETED", "Today's challenge has already been completed.") { }
}
