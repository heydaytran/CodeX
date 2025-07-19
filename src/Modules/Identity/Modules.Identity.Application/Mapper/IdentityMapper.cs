using Application.Lifetimes;
using Authentication.Entities;

namespace Modules.Identity.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class IdentityMapper : IIdentityMapper, IScoped
{
    public partial User FromDto(UserDTO userDTO);

    public partial UserDTO ToDto(User user);

    public partial void UpdateFromDto(UserDTO userDTO, User user);
}
