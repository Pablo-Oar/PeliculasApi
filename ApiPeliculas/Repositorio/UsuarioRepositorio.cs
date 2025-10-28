using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using XSystem.Security.Cryptography;

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

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);

            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = passwordEncriptado,
                Nombre = usuarioRegistroDto.Nombre,
                Rol = usuarioRegistroDto.Role
            };

            _db.Usuario.Add(usuario);
            await _db.SaveChangesAsync();
            usuario.Password = passwordEncriptado;
            return usuario;
        }

        //Metodo para encriptar password con MD5 se usa tanto en el Acceso como en el Registro.
        public static string obtenermd5(string passwordAEncriptar)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(passwordAEncriptar);
            data = x.ComputeHash(data);
            string passwordEncriptado = "";
            for (int i = 0; i < data.Length; i++)
            {
                passwordEncriptado += data[i].ToString("x2").ToLower();
            }
            return passwordEncriptado;
        }
    }
}
