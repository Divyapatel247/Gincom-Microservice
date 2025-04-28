using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
using notificationService.Services;

namespace notificationService.Consumers
{
    public class UserRegisterConsumer : IConsumer<IUserRegisterEvent>
    {
         private readonly NotificationServiceForOrderCreated _notificationService;

         public UserRegisterConsumer(NotificationServiceForOrderCreated notificationService){
           _notificationService = notificationService;
         }
        public async Task Consume(ConsumeContext<IUserRegisterEvent> context)
        {
            var message = context.Message;
            await _notificationService.NotifyUserCreated(message);
            Console.WriteLine($"[x] User {message.count} logged in at {message.count} {message.count}");
           
        } 
    }
}