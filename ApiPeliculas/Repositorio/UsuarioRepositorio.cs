using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db; // Contexto de base de datos (inyección de dependencias)
        private String secretKey;
        // Constructor que recibe el contexto y lo asigna al campo privado
        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            secretKey = config.GetValue<string>("ApiSettings:secretKey");
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

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);
            var usuario = _db.Usuario.FirstOrDefault(userDB => userDB.Password == passwordEncriptado && 
            userDB.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower());
            //Validaamos si el usuario no existe con la combinacion de usuario y password correcta
            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }
            //Aca existe el usuario entonces podemos procesar el Login
            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejadorToken.CreateToken(tokenDescriptor);
            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Token = manejadorToken.WriteToken(token),
                Usuario = usuario
            };

            return usuarioLoginRespuestaDto;
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
