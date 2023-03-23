﻿using AuthAPI.DAL.Data;
using AuthAPI.DAL.Models;
using AuthAPI.DAL.Repositories.Interfaces;
using AuthAPI.DTO;
using AuthAPI.DTO.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private AuthAPIDbContext _context;
        public UserRepository() { }
        public UserRepository(IAuthAPIDbContext context)
        {
            _context = context as AuthAPIDbContext;
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

        public async Task<IResponseDataTransferObject> GetUserAsync(IRequestDataTransferObject user)
        {
            UserRequestDataTrasferObject dataTrasferObject = user as UserRequestDataTrasferObject;
            User users = await _context.Users.Where(users => users.UserName == dataTrasferObject.UserName)
                .Where(users=>users.Password == dataTrasferObject.Password).FirstOrDefaultAsync();
            if (users == null)
            {
                throw new ArgumentException("Cannot find the user you have requested");
            }
            return new UserResponseDataTransferObject()
            {
                Id = users.Id,
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
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
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
