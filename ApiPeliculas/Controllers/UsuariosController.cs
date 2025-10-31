using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using XAct;

namespace ApiPeliculas.Controllers
{
    //[Route("api/[controller]")]  // Opción dinámica (usaría el nombre del controlador automáticamente)
    [Route("api/usuarios")] // Ruta base fija: todas las rutas comienzan con /api/peliculas
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepositorio _userRepo; // Repositorio inyectado
        private readonly IMapper _mapper; // Mapper inyectado
        protected RespuestaApi _respuestaApi;

        public UsuariosController(IUsuarioRepositorio userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            this._respuestaApi = new();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]", Name = "GetUsuarios")] // GET api/usuarios/GetUsuarios
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var listaUsuariosModel = _userRepo.GetUsuarios(); // Obtiene todos los usuarios

            var listaUsuariosDto = new List<UsuarioDto>();

            foreach (var usuarioItem in listaUsuariosModel)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(usuarioItem)); // Mapea entidad (Model) → DTO
            }

            return Ok(listaUsuariosDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]/{usuarioId:int}", Name = "GetUsuario")] // GET api/usuarios/GetUsuario
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuario(int usuarioId)
        {
            var itemUsuario = _userRepo.GetUsuario(usuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }

            var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);

            return Ok(itemUsuarioDto);
        }
        [AllowAnonymous]
        [HttpPost("[action]", Name = "Registro")] // GET api/usuarios/Registro
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            bool validarNombreUsuarioUnico = _userRepo.IsUniqueUser(usuarioRegistroDto.NombreUsuario);
            if (!validarNombreUsuarioUnico)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = await _userRepo.Registro(usuarioRegistroDto);
            if (usuario == null)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Error en el registro");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);
        }
        [AllowAnonymous]
        [HttpPost("[action]", Name = "Login")] // GET api/usuarios/Login
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {
            var respuestaLogin = await _userRepo.Login(usuarioLoginDto);

            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            _respuestaApi.Result = respuestaLogin;
            return Ok(_respuestaApi);
        }
    }
}
