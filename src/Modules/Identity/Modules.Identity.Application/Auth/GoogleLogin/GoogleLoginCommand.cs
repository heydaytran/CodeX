using Application.Messaging;
using ErrorOr;
using Modules.Identity.Application.Auth.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Identity.Application.Auth.GoogleLogin
{
    public record GoogleLoginCommand(string FullName,string Email) : ICommand<LoginResponse>;

}
