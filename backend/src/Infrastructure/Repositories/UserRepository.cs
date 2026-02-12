using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<List<string>> GetAdminEmailsAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == Role.Admin) // Filtramos solo administradores
            .Join(
                _context.UserProfiles,        // La tabla con la que queremos unir
                user => user.UserProfileId,   // La llave foránea en User
                profile => profile.Id,        // La llave primaria en UserProfiles
                (user, profile) => profile.Email // Lo que queremos extraer
            )
            .Where(email => !string.IsNullOrEmpty(email)) // Filtramos correos vacíos
            .Distinct() // Evitamos duplicados
            .ToListAsync();
    }
}