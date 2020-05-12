using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using KanarDrive.Common.Entities.Identity;
using LiteDB;
using Microsoft.AspNetCore.Identity;

namespace KanarDrive.Common.Entities
{
    public class LiteDbUserStore : IUserPasswordStore<User>, IQueryableUserStore<User>, IUserEmailStore<User>,
        IUserRoleStore<User>, IUserClaimStore<User>, IUserLockoutStore<User>
    {
        private readonly LiteDbRoleStore _roleStore;
        private readonly ILiteCollection<User> _userCollection;

        public LiteDbUserStore(ILiteDatabase db)
        {
            _roleStore = new LiteDbRoleStore(db);
            _userCollection = db.GetCollection<User>("users");
        }

        public IQueryable<User> Users => _userCollection.FindAll().AsQueryable();

        public Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            IList<Claim> claims = user.Claims != null ? user.Claims.ToList() : new List<Claim>();
            return Task.FromResult(claims);
        }

        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var currentClaims = new List<Claim>(user.Claims);
            currentClaims.AddRange(claims);
            user.Claims = currentClaims;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            var currentClaims = new List<Claim>(user.Claims);
            currentClaims.Remove(claim);
            currentClaims.Add(newClaim);
            user.Claims = currentClaims;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var currentClaims = new List<Claim>(user.Claims);
            foreach (var claim in claims) currentClaims.Remove(claim);
            user.Claims = currentClaims;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            IList<User> users = _userCollection.FindAll().Where(user => user.Claims.Contains(claim)).ToList();
            return Task.FromResult(users);
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userCollection.FindOne(x => x.NormalizedEmail == normalizedEmail));
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;
            _userCollection.Update(user);
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            user.Roles = new List<Guid>();
            user.Claims = new List<Claim>();
            _userCollection.Insert(user.Id, user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _userCollection.Update(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _userCollection.Delete(user.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        Task<User> IUserStore<User>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userCollection.FindOne(x => x.Id == userId));
        }

        Task<User> IUserStore<User>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userCollection.FindOne(x => x.NormalizedUserName == normalizedUserName));
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            _userCollection.Update(user);
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var roles = new List<Guid>(user.Roles)
            {
                await GetRoleId(roleName, cancellationToken)
            };
            user.Roles = roles;
            _userCollection.Update(user);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var roles = new List<Guid>(user.Roles);
            roles.Remove(await GetRoleId(roleName, cancellationToken));
            user.Roles = roles;
            _userCollection.Update(user);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            IList<string> roles = new List<string>();
            foreach (var roleId in user.Roles)
                roles.Add((await _roleStore.FindByIdAsync(roleId.ToString(), cancellationToken)).Name);

            return roles;
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var id = await GetRoleId(roleName, cancellationToken);
            return user.Roles.Contains(id);
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var id = await GetRoleId(roleName, cancellationToken);
            return _userCollection.FindAll().Where(user => user.Roles.Contains(id)).ToList();
        }

        private async Task<Guid> GetRoleId(string roleName, CancellationToken cancellationToken)
        {
            return Guid.Parse((await _roleStore.FindByNameAsync(roleName, cancellationToken)).Id);
        }
    }
}