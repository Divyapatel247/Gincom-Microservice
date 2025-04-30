using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace notificationService.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinAdminGroup()
        {
            Console.WriteLine($"Connection {Context.ConnectionId} joining Admin group");
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        }

        public async Task JoinUserGroup(string userId)
        {
            Console.WriteLine($"Connection {Context.ConnectionId} joining User_{userId} group");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task LeaveGroup(string groupName)
        {
            Console.WriteLine($"Connection {Context.ConnectionId} leaving {groupName} group");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

    }
}