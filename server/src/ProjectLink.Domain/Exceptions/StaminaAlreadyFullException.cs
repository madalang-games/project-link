namespace ProjectLink.Domain.Exceptions;

public class StaminaAlreadyFullException : DomainException
{
    public StaminaAlreadyFullException()
        : base("STAMINA_ALREADY_FULL", "Stamina is already at maximum.") { }
}
