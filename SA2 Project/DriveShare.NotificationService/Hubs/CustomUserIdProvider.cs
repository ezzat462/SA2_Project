using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DriveShare.NotificationService.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // Robust extraction: Check standard nameid, 'sub' claim, and the specific ClaimTypes constant
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? connection.User?.FindFirst("sub")?.Value 
                ?? connection.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
