﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeakerManagement.Data;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.ViewModels.Account;
using System.Net;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<List<UserInfo>> GetUsersList(string userRole = null);
        Task<bool> CreateUser(ApplicationUser user, string role, int organizationId, HttpRequest request);
        Task<bool> ConfirmEmail(string userId, string token);
        Task<bool> UpdateUserDetail(ApplicationUser user);
        Task<bool> DeleteUser(string userId);
    }
    #endregion

    #region class
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        #region Private
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region Constructor
        public UserRepository(
            DataContext dataContext,
            UserManager<ApplicationUser> userManager
        ) : base(dataContext)
        {
            _userManager = userManager;
        }
        #endregion

        #region Public
        public async Task<List<UserInfo>> GetUsersList(string userRole = null)
        {
            var query = from user in _context.Users
                        join user_role in _context.UserRoles on user.Id equals user_role.UserId
                        join role in _context.Roles on user_role.RoleId equals role.Id
                        join org in _context.Organizations on user.OrganizationId equals org.Id
                        select new UserInfo
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Website = user.Website,
                            OrganizationName = org.OrganizationName,
                            Role = role.Name,
                            OrganizationId = org.Id,
                            IsActive = user.EmailConfirmed
                        };

            if (userRole != null)
            {
                query = query.Where(x => x.Role.Equals(userRole));
            }

            return await query.ToListAsync();
        }
        public async Task<bool> CreateUser(ApplicationUser user, string role, int organizationId, HttpRequest request)
        {
            var adminUser = await _userManager.FindByEmailAsync(user.Email);
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = $"{user.FirstName}_{user.LastName}",
                    PhoneNumber = user.PhoneNumber,
                    OrganizationId = role == UserRoles.Speaker ? organizationId : user.OrganizationId,
                    Email = user.Email,
                    Website = user.Website
                    //EmailConfirmed = true
                };

                var autoGeneratedPassword = Common.GeneratePassword();
                
                // Create the user in the database
                var result = await _userManager.CreateAsync(newAdminUser, autoGeneratedPassword);
                if (result.Succeeded)
                {
                    // Generate email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newAdminUser);

                    // Add user to Admin role
                    await _userManager.AddToRoleAsync(newAdminUser, role);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Construct the confirmation link manually
                    var confirmationLink = $"{request.Scheme}://{request.Host}/Account/ConfirmEmail?userId={newAdminUser.Id}&token={WebUtility.UrlEncode(token)}"; //{WebUtility.UrlEncode(token)}

                    string htmlBody = $@"<html>
                                                <body>
                                                    <h1>Hello!</h1>
                                                    <p>Confirm you email to login into site</p>
                                                    <br/>
                                                    <a href='{confirmationLink}'>Confirm Email</a>
                                                    <br/>
                                                    Password: {autoGeneratedPassword}
                                                </body>
                                            </html>";

                    MailSender.SendEmailConfirmationMail(user.Email, "Email Confirmation", htmlBody, "");

                    return true;
                }
                return true;
            }
            return false;
        }

        public async Task<bool> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (userId != null && token != null)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.ConfirmEmailAsync(user, token);
                        if (result.Succeeded)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> UpdateUserDetail(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                // User not found
                return false;
            }

            // Update the user details
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.UserName = $"{user.FirstName}_{user.LastName}";
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Website = user.Website;
            existingUser.OrganizationId = user.OrganizationId;

            var result = await _userManager.UpdateAsync(existingUser);
            if (result.Succeeded)
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Delete the user using UserManager
                _ = await _userManager.DeleteAsync(user);
                return true;
            }
            return false;
        }
        #endregion
    }
    #endregion
}