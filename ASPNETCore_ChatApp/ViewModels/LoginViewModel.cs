﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_ChatApp.ViewModels
{
    public class LoginViewModel
    {
        [Required, MaxLength(256)]
        public string UserName { get; set; }

        [Required, MaxLength(256)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
