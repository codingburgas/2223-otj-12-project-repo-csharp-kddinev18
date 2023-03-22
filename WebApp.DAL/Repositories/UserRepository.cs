using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Models;
using WebApp.DAL.Repositories.Interfaces;
using WebApp.DTO;
using WebApp.DTO.Interfaces;

namespace WebApp.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IOTHomeSecurityDbContext _context;
        public UserRepository() { }
        public UserRepository(IIOTHomeSecurityDbContext context)
        {
            _context = context as IOTHomeSecurityDbContext;
        }

        public bool AddUser(IRequestDataTransferObject user, ref string errorMessage)
        {
            try
            {
                User newUser = new User(user);
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                errorMessage = "Data violation. Please check the date you have entered";
                return false;
            }
            catch (DbUpdateException ex)
            {
                errorMessage = "Data violation. Please check the date you have entered";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "General error Please check the date you have entered";
                return false;
            }

            return true;
        }

        public bool DeleteUser(int userId, ref string errorMessage)
        {
            try
            {
                User user = _context.Users.Where(user => user.Id == userId).FirstOrDefault();
                if(user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                }
                else
                    throw new ArgumentException("Cannot find the user you have requested for deletion");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                errorMessage = "Data violation. Please check the date you have entered";
                return false;
            }
            catch (DbUpdateException ex)
            {
                errorMessage = "Data violation. Please check the date you have entered";
                return false;
            }
            catch (ArgumentException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "General error Please check the date you have entered";
                return false;
            }

            return true;
        }

        public IEnumerable<IResponseDataTransferObject> Get(int pagingSize, int skipAmount, ref string errorMessage)
        {
            try
            {
                IEnumerable<User> users = _context.Users.Skip(skipAmount).Take(pagingSize);

                ICollection<UserResponseDataTransferObject> userTransferObject = new List<UserResponseDataTransferObject >();
                foreach (User user in users) 
                {
                    userTransferObject.Add(new UserResponseDataTransferObject()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email
                    });
                }

                return userTransferObject;
            }
            catch (ArgumentNullException ex)
            {
                errorMessage = "Data violation. Please check the date you have entered";
                return null;
            }
            catch (Exception ex)
            {
                errorMessage = "General error Please check the date you have entered";
                return null;
            }
        }

        public IResponseDataTransferObject GetUserById(int userId, ref string errorMessage)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(IRequestDataTransferObject user, ref string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
