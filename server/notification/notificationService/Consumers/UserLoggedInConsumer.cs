using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;

namespace notificationService.Consumers
{
    public class UserLoggedInConsumer : IConsumer<IUserLoggedInEvent>
    {
        public Task Consume(ConsumeContext<IUserLoggedInEvent> context)
        {
            var message = context.Message;
            Console.WriteLine($"[x] User {message.Username} logged in at {message.LoginTime} {message.Email}");
            // TODO: Send email or notification
            return Task.CompletedTask;
        }
    }
}