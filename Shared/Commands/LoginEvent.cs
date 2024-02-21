using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.Commands
{
    public interface IAuthenticateUser
    {
        string Email { get; set; }
        string Password { get; set; }
    }
}
