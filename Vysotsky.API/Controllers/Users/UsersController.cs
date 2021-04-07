﻿using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Infrastructure;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers.Users
{
    [Route(Resources.Users)]
    public class UsersController : ApiController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Зарегистрировать нового пользователя
        /// </summary>
        /// <param name="userProperties"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<User>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse<User>>> CreateUser([FromBody] UserProperties userProperties)
        {
            //TODO validate
            await using var _ = await _userRepository.BeginRegistrationProcedure();
            if (!await _userRepository.IsUniqueUsername(userProperties.Auth.Username))
                return BadRequest("Username is not unique", "users.registration.username.unique");

            var user = _userRepository.CreateUser(userProperties.Auth.Username, userProperties.Auth.Password);
            if (userProperties.Customer != null)
            {
                //TODO CreateCustomer
                //TODO AttachCustomer
            }

            if (userProperties.Employee != null)
            {
                //TODO CreateEmployee
                //TODO AttachEmployee
            }

            if (userProperties.Supervisor != null)
            {
                //TODO CreateSupervisor
                //TODO AttachSupervisor
            }

            return Created(Resources.Users.AppendPathSegment($"/{user.Id}"), user);
        }
    }
}