namespace Application.Interfaces;

public interface IJwtProvider
{
    string Create(Domain.Entities.User user);
}