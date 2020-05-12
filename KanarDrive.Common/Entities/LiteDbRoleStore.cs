using System.Threading;
using System.Threading.Tasks;
using KanarDrive.Common.Entities.Identity;
using LiteDB;
using Microsoft.AspNetCore.Identity;

namespace KanarDrive.Common.Entities
{
    public class LiteDbRoleStore : IRoleStore<Role>
    {
        private readonly ILiteCollection<Role> _roleCollection;

        public LiteDbRoleStore(ILiteDatabase db)
        {
            _roleCollection = db.GetCollection<Role>("roles");
        }

        public void Dispose()
        {
        }

        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            _roleCollection.Insert(role.Id, role);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            _roleCollection.Update(role);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            _roleCollection.Delete(role.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            _roleCollection.Update(role);
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            _roleCollection.Update(role);
            return Task.CompletedTask;
        }

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_roleCollection.FindOne(x => x.Id == roleId));
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_roleCollection.FindOne(x => x.NormalizedName == normalizedRoleName));
        }
    }
}