﻿using Azure.Core;
using Entities.DTO;
using Entities.Models;
using Entities.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utilities;

namespace DataAccess.ImplementServices;
public class AuthenticationServices : IAuthenticationServices
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JWT _jwt;

    public AuthenticationServices
        (UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,IOptions<JWT> jwt)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._jwt = jwt.Value;
    }
   
    //-----------------------------------------------------------------------------------------
    public async Task<AuthenticationModel> RegisterAsync(RegisterDTO model)
    {
        if ((await _userManager.FindByNameAsync(model.UserName)) is not null)
            return new AuthenticationModel { Message = "UserName is already register" };


        if ((await _userManager.FindByEmailAsync(model.Email)) is not null)
            return new AuthenticationModel { Message="Email is already register"};
        

        ApplicationUser user = new()
        {
            UserName = model.UserName,
            Email = model.Email,
        };

        IdentityResult result=await _userManager.CreateAsync(user,model.Password);
                                                                                                                            
        if (!result.Succeeded)
        {
            StringBuilder errors = new StringBuilder();
            foreach (var error in result.Errors)
                errors.Append($"{error.Description},");

            return new AuthenticationModel { Message = errors.ToString() };
        }

        if (!(await _roleManager.RoleExistsAsync(SD.AdminRole)))
        {
            await _roleManager.CreateAsync(new IdentityRole(SD.AdminRole));
            await _roleManager.CreateAsync(new IdentityRole(SD.DoctorRole));
            await _roleManager.CreateAsync(new IdentityRole(SD.UserRole));
            await _userManager.AddToRoleAsync(user, SD.AdminRole);
        }
        else
        {
            if (model.IsDoctor)
                await _userManager.AddToRoleAsync(user, SD.DoctorRole);
            else
                await _userManager.AddToRoleAsync(user, SD.UserRole);
        }

        var jwtSecurityToken = await CreateJwtToken(user);
        var UserRoles = await _userManager.GetRolesAsync(user);

        return new AuthenticationModel {
            Email=user.Email,
            UserName=user.UserName,
            ExoiresOn=jwtSecurityToken.ValidTo,
            IsAuthenticated=true,
            Roles= UserRoles.ToList(),
            Token=new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
        };
    }
    //-----------------------------------------------------------------------------------------
    public async Task<AuthenticationModel> GetTokenAsync(TokenRequstModel model)
    {
        AuthenticationModel authModel = new();
        ApplicationUser applicationUser = (await _userManager.FindByEmailAsync(model.Email));
        if (applicationUser is null || !(await _userManager.CheckPasswordAsync(applicationUser, model.Password)))
        {
            authModel.Message = "Email or password is wrong";
            return authModel;
        }

        var jwtSecurityToken = await CreateJwtToken(applicationUser);
        var UserRoles = await _userManager.GetRolesAsync(applicationUser);

        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authModel.IsAuthenticated = true;
        authModel.Email = applicationUser.Email;
        authModel.UserName = applicationUser.UserName;
        authModel.ExoiresOn = jwtSecurityToken.ValidTo;
        authModel.Roles = UserRoles.ToList();

        return authModel;
    }
    //-----------------------------------------------------------------------------------------
    //public async Task<string> AddRoleAsync(AddRoleModel model)
    //{
    //    var user=await _userManager.FindByIdAsync(model.UserId);
    //    if (user is null || !(await _roleManager.RoleExistsAsync(model.Role)))
    //        return "User or Role Not Found";

    //    if(await _userManager.IsInRoleAsync(user,model.Role))
    //        return "User already assigned to this role";

    //    IdentityResult result = await _userManager.AddToRoleAsync(user,model.Role);

    //    return (result.Succeeded?string.Empty:"Something went wrong!");

    //}
    //-----------------------------------------------------------------------------------------
    public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var UserClaims=await _userManager.GetClaimsAsync(user);
        var UserRoles=await _userManager.GetRolesAsync(user);
        var RolesClaims = new List<Claim>();
        foreach (var role in UserRoles)
        {
            RolesClaims.Add(new Claim(ClaimTypes.Role, role));
        }
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uId", user.Id)
        }.Union(RolesClaims).Union(UserClaims);


        SymmetricSecurityKey SignInKey = new(Encoding.UTF8.GetBytes(_jwt.SecritKey));

        SigningCredentials signingCreds = new(SignInKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.IssuerIP,
            audience: _jwt.AudienceIP,
            expires: DateTime.Now.AddDays(_jwt.DurationInDays),
            claims: claims,
            signingCredentials: signingCreds
        );
        return jwtSecurityToken;
    }
    //-----------------------------------------------------------------------------------------

}