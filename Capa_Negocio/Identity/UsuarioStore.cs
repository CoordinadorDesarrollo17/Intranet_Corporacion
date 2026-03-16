using Capa_Datos.UsuarioD;
using Capa_Modelo.Login;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Identity
{
    public class UsuarioStore : IUserStore<Usuario>, IUserPasswordStore<Usuario>
    {
        private readonly IUsuarioDatos usuarioDatos;

        public UsuarioStore(IUsuarioDatos usuarioDatos)
        {
            this.usuarioDatos = usuarioDatos;
        }
        public async Task<IdentityResult> CreateAsync(Usuario user, CancellationToken cancellationToken) //modificado
        {
            user.IdUsuario = await usuarioDatos.CrearUsuario(user);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(Usuario user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<Usuario?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Usuario?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) //modificado
        {
            return await usuarioDatos.BuscarUsuario(normalizedUserName);
        }

        public Task<string?> GetNormalizedUserNameAsync(Usuario user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetPasswordHashAsync(Usuario user, CancellationToken cancellationToken)//MODIFICADO
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(Usuario user, CancellationToken cancellationToken) //modificado
        {
            return Task.FromResult(user.IdUsuario.ToString());
        }

        public Task<string?> GetUserNameAsync(Usuario user, CancellationToken cancellationToken) //Modificado
        {
            return Task.FromResult(user.UsuarioConcatenado);
        }

        public Task<bool> HasPasswordAsync(Usuario user, CancellationToken cancellationToken) //modificado
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetNormalizedUserNameAsync(Usuario user, string? normalizedName, CancellationToken cancellationToken) //modificado
        {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(Usuario user, string? passwordHash, CancellationToken cancellationToken) // modificado
        {

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(Usuario user, string? userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> UpdateAsync(Usuario user, CancellationToken cancellationToken) //MODIFICADO
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Llamamos a la capa de datos para actualizar
            var filasAfectadas = await usuarioDatos.ActualizarUsuario(user);

            return filasAfectadas > 0
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError
                {
                    Description = "No se pudo actualizar el usuario."
                });
        }
    }
}
