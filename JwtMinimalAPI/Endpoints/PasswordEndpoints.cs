﻿using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Services;

namespace JwtMinimalAPI.Endpoints
{
    public static class PasswordEndpoints
    {
        public static void GetPasswordEndpoints(WebApplication app)
        {
            app.MapPost("/change-password", ChangeUserPassword);
            //app.MapPost("/reset-password", ResetPassword);
        }
        private static async Task<IResult> ChangeUserPassword(PasswordService change, ChangePasswordDto dto)
        {
            var changedPassword = await change.ChangedPassword(dto);

            if (changedPassword)
            {
                return Results.Ok("Password changed");
            }
            else
            {
                return Results.BadRequest();
            }
        }
        private static async Task<IResult> ForgotPassword(PasswordService forgot, ResetPasswordDto dto)
        {
            return Results.Ok("Password changed");
        }
    }
}
