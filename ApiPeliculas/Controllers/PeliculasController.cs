using ApiPeliculas.Modelos; // Importa las clases del modelo principal (entidades)
using ApiPeliculas.Modelos.DTOs; // Importa los DTOs (objetos de transferencia de datos)
using ApiPeliculas.Repositorio.IRepositorio; // Importa las interfaces de los repositorios
using AutoMapper; // Importa AutoMapper para mapear entre entidades y DTOs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; // Para usar los controladores y atributos MVC

namespace ApiPeliculas.Controllers
{
    //[Route("api/[controller]")]  // Opción dinámica (usaría el nombre del controlador automáticamente)
    [Route("api/peliculas")] // Ruta base fija: todas las rutas comienzan con /api/peliculas
    [ApiController] // Habilita validación automática de modelos y comportamientos de API
    public class PeliculasController : ControllerBase // Hereda de ControllerBase (sin vistas, solo API)
    {
        private readonly IPeliculaRepositorio _pelRepo; // Referencia al repositorio de películas
        private readonly IMapper _mapper; // Referencia a AutoMapper para convertir entre entidades y DTOs

        public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper)
        {
            _pelRepo = pelRepo; // Inyección del repositorio
            _mapper = mapper; // Inyección del mapper
        }
        [AllowAnonymous]
        [HttpGet("[action]", Name = "GetPeliculas")] // GET api/peliculas/GetPeliculas
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Documenta posibles respuestas
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _pelRepo.GetPeliculas(); // Obtiene todas las películas del repositorio
             
            var listaPeliculasDto = new List<PeliculaDto>(); // Crea una lista para los DTOs

            foreach (var peliculaItem in listaPeliculas) // Recorre todas las películas
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(peliculaItem)); // Convierte cada entidad en DTO
            }

            return Ok(listaPeliculasDto); // Devuelve 200 con la lista en formato DTO
        }
        [AllowAnonymous]
        [HttpGet("[action]/{peliculaId:int}", Name = "GetPelicula")] // GET api/peliculas/GetPelicula/{peliculaId}
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _pelRepo.GetPelicula(peliculaId); // Busca la película por ID

            if (itemPelicula == null)
            {
                return NotFound();
            }

            var itemPeliculaDto = _mapper.Map<PeliculaDto>(itemPelicula); // Convierte la entidad a DTO

            return Ok(itemPeliculaDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("[action]", Name = "CrearPelicula")] // POST api/peliculas/CrearPelicula
        [ProducesResponseType(201, Type = typeof(PeliculaDto))] // Define tipos de respuesta esperados
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearPelicula([FromBody] CrearPeliculaDto crearPeliculaDto)
        {
            if (!ModelState.IsValid) // Valida el modelo recibido
            {
                return BadRequest(ModelState);
            }

            if (crearPeliculaDto == null) // Verifica que el objeto no sea nulo
            {
                return BadRequest(ModelState);
            }

            if (_pelRepo.ExistePelicula(crearPeliculaDto.Nombre)) // Evita duplicados por nombre
            {
                ModelState.AddModelError("", $"La pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(crearPeliculaDto); // Mapea DTO → entidad

            if (!_pelRepo.CrearPelicula(pelicula)) // Guarda la nueva película
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {pelicula.Nombre}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("CrearPelicula", new { peliculaId = pelicula.Id }, pelicula); // Retorna 201 con la URL del recurso creado
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("[action]/{peliculaId:int}", Name = "ActualizarPatchPelicula")] // PATCH api/peliculas/ActualizarPelicula/{peliculaId}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchPelicula(int peliculaId, [FromBody] PeliculaDto peliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (peliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (peliculaDto == null || peliculaId != peliculaDto.Id) // IDs deben coincidir
            {
                return BadRequest(ModelState);
            }

            var peliculaExistente = _pelRepo.GetPelicula(peliculaId); // Verifica existencia
            if (peliculaExistente == null)
            {
                return NotFound($"No se encontro la pelicula con Id {peliculaId}");
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto); // Convierte DTO → entidad

            if (!_pelRepo.ActualizarPelicula(pelicula)) // Actualiza la película
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }
             
            return NoContent(); // Devuelve 204 sin contenido
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]/{peliculaId:int}", Name = "BorrarPelicula")] // DELETE api/peliculas/BorrarPelicula/{peliculaId}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarPelicula(int peliculaId)
        {

            if (!_pelRepo.ExistePelicula(peliculaId))
            {
                return BadRequest(ModelState);
            }

            var pelicula = _pelRepo.GetPelicula(peliculaId); // Recupera la película

            if (!_pelRepo.BorrarPelicula(pelicula)) // Intenta eliminarla
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        [AllowAnonymous]
        [HttpGet("[action]/{categoriaId:int}", Name = "GetPeliculasEnCategoria")] // GET api/peliculas/GetPeliculasEnCategoria/{categoriaId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPeliculas = _pelRepo.GetPeliculasEnCategoria(categoriaId); // Obtiene películas filtradas por categoría

            if (listaPeliculas == null)
            {
                return NotFound(); // Si no hay resultados, devuelve 404
            }

            var itemPelicula = new List<PeliculaDto>(); // Lista de DTOs
            foreach (var pelicula in listaPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(pelicula)); // Mapea entidad → DTO
            }

            return Ok(itemPelicula); // Devuelve 200 con la lista
        }
        [AllowAnonymous]
        [HttpGet("[action]", Name = "BuscarPelicula")] // GET api/peliculas/BuscarPelicula/{nombre}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BuscarPelicula(string nombre)
        {
            try
            {
                var resultado = _pelRepo.BuscarPelicula(nombre); // Ejecuta búsqueda por nombre
                if (resultado.Any())  // Si hay coincidencias
                {
                    return Ok(resultado); // Devuelve 200 con resultados
                }
                return NotFound(); // Si no hay, 404
            }
            catch (Exception) // Manejo general de errores
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicacion.");
            }
          
        }
    }
}
