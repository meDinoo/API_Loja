using Loja_Api.Model;
using Loja_Api.Model.Auth;

namespace Loja_Api.Interfaces
{
    public interface IAuthManager
    {
        Task<IEnumerable<Usuario>> GetAll();
        Task<Usuario> Post(Usuario usuario);
        Task<Usuario> Update(Usuario usuario);
        Task<bool> Delete(Guid id);
        Task<UsuarioConta> Cadastrar(CadastroUsuario cadastro);
        Task<LoginResposta?> Login(LoginUsuario credenciais);
    }
}
