using Authentication.Entities;
using Modules.Identity.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Identity.Application.Mapper
{
    public interface IIdentityMapper
    {
        UserDTO ToDto(User customer);
        User FromDto(UserDTO userDTO);
        void UpdateFromDto(UserDTO userDTO, User user);
    }
}
