﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.Response
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public string TokenExpiry { get; set; }
    }
}
