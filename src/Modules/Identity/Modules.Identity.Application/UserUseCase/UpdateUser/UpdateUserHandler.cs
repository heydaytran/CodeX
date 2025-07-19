
using Authentication.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Modules.Identity.Application.Mapper;
using Modules.Identity.Application.Signup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Modules.Identity.Application.UserUseCase.UpdateUser
{
    public class UpdateUserHandler(UserManager<User> userManager,
        IIdentityMapper identityMapper) : ICommandHandler<UpdateUserCommand, UserDTO>
    {

        private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly IIdentityMapper _identityMapper = identityMapper ?? throw new ArgumentNullException(nameof(identityMapper));


        public async Task<ErrorOr<UserDTO>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserDTO.Id.ToString());

            // Update logic
            if (user is null)
            {
                return Error.NotFound("user not found");
            }
            else
            {
                //_identityMapper.UpdateFromDto(request.UserDTO, user); // use is updated with dto --> entity status was changed to
                 user.Email = request.UserDTO.Email;
                 user.UserName = request.UserDTO.UserName;
                 user.PhoneNumber = request.UserDTO.PhoneNumber;
                 user.TwoFactorEnabled = request.UserDTO.TwoFactorEnabled;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Error.Validation("user update", errors);
            }

            return request.UserDTO;
        }
    }
}
