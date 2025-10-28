using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db; // Contexto de base de datos (inyección de dependencias)

        // Constructor que recibe el contexto y lo asigna al campo privado
        public UsuarioRepositorio(ApplicationDbContext db)
        {
            _db = db;
        }
        public Usuario GetUsuario(int usuarioId)
        {
            return _db.Usuario.FirstOrDefault(usId => usId.Id == usuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
             return _db.Usuario.OrderBy(lUsers => lUsers.Nombre ).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuariBd = _db.Usuario.FirstOrDefault(usBd => usBd.NombreUsuario == usuario);
            if (usuariBd == null)
            {
                return false;
            }
            return true;
        }

        public Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            throw new NotImplementedException();
        }
    }
}
