﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Enums
{
    public enum TokenType
    {
        TwoFactorAuth = 1,
        EmailValidtion,
        PhoneValidtion,
        PasswordReset
    }
}
