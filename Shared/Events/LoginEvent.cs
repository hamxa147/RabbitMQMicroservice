using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.Events
{
    public interface ILoginEvent
    {
        string Email { get; set; }
        string Password { get; set; }
    }
}
