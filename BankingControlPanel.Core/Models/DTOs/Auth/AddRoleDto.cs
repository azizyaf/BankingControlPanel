﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.DTOs.Auth
{
    public class AddRoleDto
    {
        [Required(ErrorMessage = "Role name is required")]
        public string Name { get; set; }
    }

}
