using IglesiaNet.Domain.Users;

namespace IglesiaNet.Application.Common;

public interface IJwtTokenProvider
{
    string Generate(User user);
}
