using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
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
    }
}
