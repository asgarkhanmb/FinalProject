﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ISendEmail
    {
        void Send(string from, string displayName, string to, string messageBody, string subject);
    }
}
