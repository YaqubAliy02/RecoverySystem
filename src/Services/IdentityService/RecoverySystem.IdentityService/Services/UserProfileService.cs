using Microsoft.EntityFrameworkCore;
using RecoverySystem.IdentityService.Data;
using RecoverySystem.IdentityService.Models;

namespace RecoverySystem.IdentityService.Services;

public class UserProfileService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(ApplicationDbContext context, ILogger<UserProfileService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserProfile> GetUserProfileAsync(string userId)
    {
        var profile = await _context.UserProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        return profile;
    }

    public async Task<UserProfile> CreateUserProfileAsync(UserProfile profile)
    {
        profile.Id = Guid.NewGuid().ToString();
        profile.CreatedAt = DateTime.UtcNow;
        profile.UpdatedAt = DateTime.UtcNow;

        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created user profile for user {UserId}", profile.UserId);
        return profile;
    }

    public async Task<UserProfile> UpdateUserProfileAsync(UserProfile profile)
    {
        var existingProfile = await _context.UserProfiles.FindAsync(profile.Id);
        if (existingProfile == null)
        {
            _logger.LogWarning("Profile not found for ID {ProfileId}", profile.Id);
            return null;
        }

        existingProfile.Department = profile.Department;
        existingProfile.Position = profile.Position;
        existingProfile.ContactNumber = profile.ContactNumber;
        existingProfile.Address = profile.Address;
        existingProfile.Bio = profile.Bio;
        existingProfile.Specializations = profile.Specializations;
        existingProfile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated user profile for user {UserId}", profile.UserId);

        return existingProfile;
    }
}