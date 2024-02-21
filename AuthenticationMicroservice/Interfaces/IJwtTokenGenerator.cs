using AuthenticationMicroservice.Entities;

namespace AuthenticationMicroservice.Interfaces
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(User user);
    }
}
