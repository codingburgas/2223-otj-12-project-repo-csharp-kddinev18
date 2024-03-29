﻿using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using BridgeAPI.DAL.Data;
using BridgeAPI.DAL.Models;
using BridgeAPI.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private BridgeAPIDbContext _context;
        public UserRepository() { }
        public UserRepository(IBridgeAPIDbContext context)
        {
            _context = context as BridgeAPIDbContext;
        }

        public async Task<bool> AddUserAsync(IRequestDataTransferObject user)
        {
            User newUser = new User(user);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            User user = await _context.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("Cannot find the user you have requested for deletion");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<byte[]> GetUserImage(Guid userId)
        {
            User user = await _context.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("Cannot find the user you have requested for deletion");
            }
            return user.Image;
        }

        public async Task<IResponseDataTransferObject> GetUserAsync(IRequestDataTransferObject user)
        {
            UserRequestDataTransferObject dataTrasferObject = user as UserRequestDataTransferObject;
            User users = await _context.Users.Where(users => users.UserName == dataTrasferObject.UserName)
                .Where(users => users.Password == dataTrasferObject.Password).FirstOrDefaultAsync();
            if (users == null)
            {
                throw new ArgumentException("Wrong credentials");
            }
            return new UserResponseDataTransferObject()
            {
                GlobalServerId = users.Id,
                UserName = users.UserName,
                Email = users.Email
            };
        }

        public async Task<IResponseDataTransferObject> GetUserByIdAsync(Guid userId)
        {
            User user = await _context.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("Cannot find the user you have requested");
            }
            return new UserResponseDataTransferObject()
            {
                GlobalServerId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<string> GetUserSalt(IRequestDataTransferObject user)
        {
            UserRequestDataTransferObject dataTrasferObject = user as UserRequestDataTransferObject;
            User userObject = await _context.Users
                .Where(user => user.UserName == dataTrasferObject.UserName)
                .FirstOrDefaultAsync();
            if(userObject == null) { throw new ArgumentNullException("Cannot find the user"); }
            return userObject.Salt;
        }

        public async Task<bool> UpdateUserAsync(Guid userId)
        {
            User user = await _context.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("Cannot find the user you have requested for deletion");
            }
            user.UserName = user.UserName;
            user.Email = user.Email;
            user.Password = user.Password;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
