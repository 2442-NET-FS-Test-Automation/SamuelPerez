using Library.Data;
using Library.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.ControllerApi.Services;

public class UserService : IUserService
{
    private readonly LibraryDbContext _db;
    private readonly IPasswordHasher<User> _hasher;
    public UserService(LibraryDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<string?> RegisterAsync(string username, string password)
    {
        string name = username.Trim();

        if (await _db.Users.AnyAsync(u => u.UserName == name)) return "username is taken";

        User newUser = new User {UserName = name, Role = "consumer"};

        newUser.PasswordHash = _hasher.HashPassword(newUser, password);

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        return null;
    }

    public async Task<User?> ValidateAsync(string username, string password)
    {
        User? foundUser = await _db.Users.SingleOrDefaultAsync(u => u.UserName == username);

        if (foundUser is null) return null;

        var result = _hasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash, password);

        return result == PasswordVerificationResult.Failed ? null : foundUser;
    }
}