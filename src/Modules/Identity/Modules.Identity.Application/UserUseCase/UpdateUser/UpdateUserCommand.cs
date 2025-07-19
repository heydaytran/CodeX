using Application.Messaging;
using ErrorOr;
using Modules.Identity.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Identity.Application.UserUseCase.UpdateUser;

public class UpdateUserCommand : ICommand<UserDTO>
{
   public UserDTO UserDTO { get; set; } = new UserDTO();
}
