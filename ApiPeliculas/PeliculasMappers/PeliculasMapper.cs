using ApiPeliculas.Modelos.DTOs;
using AutoMapper;

namespace ApiPeliculas.PeliculasMappers
{
    public class PeliculasMapper : Profile
    {
        public PeliculasMapper() {
            CreateMap<CategoriaRepositorio, CategoriaDto>().ReverseMap();
            CreateMap<CategoriaRepositorio, CrearCategoriaDto>().ReverseMap();
        }
    }
}
