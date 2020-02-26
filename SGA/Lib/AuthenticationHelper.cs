using Microsoft.AspNetCore.Authorization;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGA.Lib
{
    public class AuthenticationHelper
    {
        public static List<Claim> GetClaimType(EnumSGA.AccessType Permission)
        {
            List<Claim> claims = new List<Claim>();

            switch (Permission)
            {
                case EnumSGA.AccessType.Administration:
                    claims.Add(new Claim("Public", "true"));
                    claims.Add(new Claim("Administration", "true"));
                    claims.Add(new Claim("Report", "true"));
                    claims.Add(new Claim("Log", "true"));
                    claims.Add(new Claim("UserManagementRead", "true"));
                    claims.Add(new Claim("UserManagementWrite", "true"));
                    claims.Add(new Claim("Manager", "true"));
                    claims.Add(new Claim("HR", "true"));
                    break;
                case EnumSGA.AccessType.Reports:
                    claims.Add(new Claim("Public", "true"));
                    claims.Add(new Claim("Report", "true"));
                    break;
                case EnumSGA.AccessType.Logs:
                    claims.Add(new Claim("Public", "true"));
                    claims.Add(new Claim("Log", "true"));
                    break;
                case EnumSGA.AccessType.UserManagementRead:
                    claims.Add(new Claim("Public", "true"));
                    claims.Add(new Claim("UserManagementRead", "true"));
                    claims.Add(new Claim("Report", "true"));
                    break;
                case EnumSGA.AccessType.UserManagementWrite:
                    claims.Add(new Claim("Public", "true"));
                    claims.Add(new Claim("Report", "true"));
                    claims.Add(new Claim("UserManagementRead", "true"));
                    claims.Add(new Claim("UserManagementWrite", "true"));
                    claims.Add(new Claim("HR", "true"));
                    claims.Add(new Claim("Log", "true"));
                    break;
                case EnumSGA.AccessType.Manager:
                    claims.Add(new Claim("Public", "true"));
                    claims.Add(new Claim("Manager", "true"));
                    claims.Add(new Claim("Report", "true"));
                    break;
                case EnumSGA.AccessType.HR:
                    claims.Add(new Claim("Public", "true"));
                    claims.Add(new Claim("Report", "true"));
                    claims.Add(new Claim("HR", "true"));
                    claims.Add(new Claim("UserManagementRead", "true"));
                    break;
            }

            return claims;
        }
        
        public static void GetAuthorizationOptions(AuthorizationOptions options)
        {
            options.AddPolicy("Administration", policy => policy.RequireClaim("Administration"));
            options.AddPolicy("Report", policy => policy.RequireClaim("Report"));
            options.AddPolicy("Log", policy => policy.RequireClaim("Log"));
            options.AddPolicy("UserManagementRead", policy => policy.RequireClaim("UserManagementRead"));
            options.AddPolicy("UserManagementWrite", policy => policy.RequireClaim("UserManagementWrite"));
            options.AddPolicy("Manager", policy => policy.RequireClaim("Manager"));
            options.AddPolicy("HR", policy => policy.RequireClaim("HR"));
            options.AddPolicy("Public", policy => policy.RequireClaim("Public"));
        }
    }
}
