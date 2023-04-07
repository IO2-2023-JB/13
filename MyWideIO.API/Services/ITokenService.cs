﻿using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Services
{
    public interface ITokenService
    {
        public string GenerateToken(ViewerModel viewer, IList<string> roles);
    }
}
