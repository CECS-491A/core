﻿using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DTO.UserManagementAPI;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.UserManagement
{
    public class UserManagementManager
    {
        DatabaseContext _db;
        UserService _userService;

        public UserManagementManager(DatabaseContext db)
        {
            _db = db;
            _userService = new UserService(_db);
        }

        public User CreateUser(string email, Guid SSOID)
        {
            try
            {
                var useremail = new System.Net.Mail.MailAddress(email);
                var existingUserWithEmail = _userService.GetUser(email);
                if (existingUserWithEmail != null)
                {
                    throw new UserAlreadyExistsException("User with that email already exists.");
                }
            }
            catch (FormatException e)
            {
                throw new InvalidEmailException("Invalid Email", e);
            }
            var _passwordService = new PasswordService();
            DateTime timestamp = DateTime.UtcNow;
            byte[] salt = _passwordService.GenerateSalt();
            string hash = _passwordService.HashPassword(timestamp.ToString(), salt);
            User user = new User
            {
                Username = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UpdatedAt = timestamp,
                Id = SSOID
            };
             _userService.CreateUser(user);
            return user;
        }

        public User CreateUser(CreateUserRequestDTO user)
        {
            // update user with given values
            try
            {
                var newUserUsername = new System.Net.Mail.MailAddress(user.Username);
                var newUserId = Guid.NewGuid();
                var newUser = CreateUser(user.Username, newUserId);
                if (user.Manager != "")
                {
                    try
                    {
                        var managerId = Guid.Parse(user.Manager);
                        var manager = _userService.GetUser(managerId);
                        if (manager == null)
                        {
                            throw new UserNotFoundException("Manager does not exist.");
                        }
                        newUser.ManagerId = managerId;
                    }
                    catch (FormatException e)
                    {
                        throw new InvalidGuidException("Invalid Manager ID.", e);
                    }
                }
                newUser.IsAdministrator = user.IsAdmin;
                newUser.City = user.City;
                newUser.Country = user.Country;
                newUser.State = user.State;
                newUser.Disabled = user.Disabled;
                // update new user with changes
                _userService.CreateUser(newUser);
                return newUser;
            }
            catch (FormatException)
            {
                throw new InvalidEmailException("Invalid Username.");
            }
        }

        public void DeleteUser(Guid id)
        {
            var user = _userService.GetUser(id);
            if (user == null)
            {
                throw new UserNotFoundException("User does not exist.");
            }
            var _sessionService = new SessionService(_db);
            _sessionService.DeleteSessionsOfUser(id);
            _userService.DeleteUser(id);
        }

        public User ToUpdateUser(User user, UpdateUserRequestDTO requestChanges)
        {
            if (user == null)
            {
                throw new UserNotFoundException("User does not exist.");
            }
            user.City = requestChanges.City;
            user.State = requestChanges.State;
            user.Country = requestChanges.Country;
            user.Disabled = requestChanges.Disabled;
            user.IsAdministrator = requestChanges.IsAdmin;
            user.ManagerId = null;
            if (requestChanges.Manager != "" && requestChanges.Manager != null)
            {
                // check if manager id is valid ssoid
                Guid managerId;
                var validParse = Guid.TryParse(requestChanges.Manager, out managerId);
                if (!validParse)
                {
                    throw new InvalidGuidException("Invalid Manager Id.");
                }
                var manager = GetUser(managerId);
                if (manager == null)
                {
                    throw new UserNotFoundException("Manager does not exist.");
                }
                user.ManagerId = managerId;
            }
            return user;
        }

        public User GetUser(Guid id)
        {
            var user = _userService.GetUser(id);
            return user;
        }

        public User GetUser(string email)
        {
            var user = _userService.GetUser(email);
            return user;
        }

        public List<GetAllUsersResponseDataItem> GetUsers()
        {
            var users = _db.Users
                .Select(u => new GetAllUsersResponseDataItem
                {
                    id = u.Id,
                    username = u.Username,
                    manager = u.ManagerId,
                    city = u.City,
                    state = u.State,
                    country = u.Country,
                    disabled = u.Disabled,
                    isAdmin = u.IsAdministrator
                }).ToList();
            return users;
        }

        public void DisableUser(User user)
        {
            ToggleUser(user, true);
        }

        public void EnableUser(User user)
        {
            ToggleUser(user, false);
        }

        public void ToggleUser(User user, bool? disable)
        {
            if (disable == null)
            {
                disable = !user.Disabled;
            }
            user.Disabled = (bool)disable;
            _userService.UpdateUser(user);
        }

        public void UpdateUser(User user)
        {
            _userService.UpdateUser(user);
        }
    }
}
