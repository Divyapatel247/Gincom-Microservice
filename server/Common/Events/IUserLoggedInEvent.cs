using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Events
{
    public interface IUserLoggedInEvent
    {
        string UserId { get; }
        string Username { get; }

        string Email { get; }
        DateTime LoginTime { get; }
    }
}