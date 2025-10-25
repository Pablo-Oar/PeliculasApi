using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using AutoMapper;

namespace ApiPeliculas.PeliculasMappers
{
    public class PeliculasMapper : Profile
    {
        //Sirve para mapear cada entidad con los diferentes DTOs correspondientes y permite el mapeo bidireccional.
        public PeliculasMapper() {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Pelicula, CrearPeliculaDto>().ReverseMap();
        }
    }
}
