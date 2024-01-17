using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OctaShared.Contracts;
using OctaShared.Events;

namespace Auth.EventHandlers.Customer;

public class CustomerEventHandler : IEventHandler<CustomerCreatedEvent>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    public CustomerEventHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task HandleAsync(CustomerCreatedEvent @event)
    {
        var user = await _context.Users.FirstOrDefaultAsync(a => a.PhoneNumber == @event.PhoneNumber);
        if (user == null)
        {
            ApplicationUser appUser = new()
            {
                PhoneNumber = @event.PhoneNumber,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                UserName = @event.Code.ToString()
            };
            await _userManager.CreateAsync(appUser, @event.PhoneNumber);//todo:totp later
            await _userManager.AddToRoleAsync(user, "Customer");
        }
    }
}
