namespace SmartDiscount.Ordering.Domain.Exceptions;

/// <summary>
/// Exception type domain exceptions
/// </summary>

public class OrderingDomainException : Exception
{
    public OrderingDomainException()
    {

    }

    public OrderingDomainException(string message) : base(message) 
    {

    }

    public OrderingDomainException(string message, Exception innerException) : base(message, innerException) 
    {

    }
}