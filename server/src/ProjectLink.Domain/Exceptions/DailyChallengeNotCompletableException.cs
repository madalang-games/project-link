namespace ProjectLink.Domain.Exceptions;

public class DailyChallengeNotCompletableException : DomainException
{
    public DailyChallengeNotCompletableException()
        : base("DAILY_CHALLENGE_NOT_COMPLETABLE", "Play count target has not been reached yet.") { }
}
