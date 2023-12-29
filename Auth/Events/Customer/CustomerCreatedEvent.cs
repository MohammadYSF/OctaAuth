using Auth.Core;

namespace Auth.Events.Customer;

public class CustomerCreatedEvent : DomainEvent
{
    public CustomerCreatedEvent() : base(nameof(CustomerCreatedEvent))
    {
    }
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }

    public int Code { get; set; }
}
