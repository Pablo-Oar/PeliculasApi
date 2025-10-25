using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace ApiPeliculas.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepositorio _pelRepo;
        private readonly IMapper _mapper;

        public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper)
        {
            _pelRepo = pelRepo;
            _mapper = mapper;
        }

        [HttpGet("[action]", Name = "GetPeliculas")] // GET api/peliculas/GetPeliculas
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _pelRepo.GetPeliculas();

            var listaPeliculasDto = new List<PeliculaDto>();

            foreach (var peliculaItem in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(peliculaItem));
            }

            return Ok(listaPeliculasDto);
        }
        [HttpGet("[action]/{peliculaId:int}", Name = "GetPelicula")] // GET api/peliculas/GetPelicula/{peliculaId}
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _pelRepo.GetPelicula(peliculaId);

            if (itemPelicula == null)
            {
                return NotFound();
            }

            var itemPeliculaDto = _mapper.Map<PeliculaDto>(itemPelicula);

            return Ok(itemPeliculaDto);
        }
        [HttpPost("[action]", Name = "CrearPelicula")] // POST api/peliculas/CrearPelicula
        [ProducesResponseType(201, Type = typeof(PeliculaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearPelicula([FromBody] CrearPeliculaDto crearPeliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (crearPeliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_pelRepo.ExistePelicula(crearPeliculaDto.Nombre))
            {
                ModelState.AddModelError("", $"La pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(crearPeliculaDto);

            if (!_pelRepo.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {pelicula.Nombre}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
        }
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

            if (peliculaDto == null || peliculaId != peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var peliculaExistente = _pelRepo.GetPelicula(peliculaId);
            if (peliculaExistente == null)
            {
                return NotFound($"No se encontro la pelicula con Id {peliculaId}");
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
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

            var pelicula = _pelRepo.GetPelicula(peliculaId);

            if (!_pelRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        [HttpGet("[action]/{categoriaId:int}", Name = "GetPeliculasEnCategoria")] // GET api/peliculas/GetPeliculasEnCategoria/{categoriaId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPeliculas = _pelRepo.GetPeliculasEnCategoria(categoriaId);

            if (listaPeliculas == null)
            {
                return NotFound();
            }

            var itemPelicula = new List<PeliculaDto>();
            foreach (var pelicula in listaPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(pelicula));
            }

            return Ok(itemPelicula);
        }
        [HttpGet("[action]", Name = "BuscarPelicula")] // GET api/peliculas/BuscarPelicula/{nombre}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BuscarPelicula(string nombre)
        {
            try
            {
                var resultado = _pelRepo.BuscarPelicula(nombre);
                if (resultado.Any())
                {
                    return Ok(resultado);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicacion.");
            }
          
        }
    }
}
