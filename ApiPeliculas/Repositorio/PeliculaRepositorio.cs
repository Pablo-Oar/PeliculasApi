using ApiPeliculas.Data; // Importa el contexto de base de datos
using ApiPeliculas.Modelos; // Importa las entidades del modelo (Pelicula)
using ApiPeliculas.Repositorio.IRepositorio; // Importa la interfaz del repositorio
using Microsoft.EntityFrameworkCore; // Importa funciones de Entity Framework Core

namespace ApiPeliculas.Repositorio
{
    // Implementa la interfaz IPeliculaRepositorio
    public class PeliculaRepositorio : IPeliculaRepositorio
    {
        private readonly ApplicationDbContext _db; // Contexto de base de datos (inyección de dependencias)

        // Constructor que recibe el contexto y lo asigna al campo privado
        public PeliculaRepositorio(ApplicationDbContext db)
        {
            _db = db;
        }
        // Actualiza una película existente en la base de datos
        public bool ActualizarPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now; // Actualiza la fecha de creación          
            _db.Update(pelicula); // Marca la entidad como modificada           
            return Guardar(); // Guarda los cambios y devuelve el resultado
        }

        // Elimina una película de la base de datos
        public bool BorrarPelicula(Pelicula pelicula)
        {
            _db.Pelicula.Remove(pelicula);// Elimina la entidad del contexto
            return Guardar(); // Guarda los cambios
        }

        // Busca películas por nombre o descripción parcial
        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _db.Pelicula; // Base de la consulta
            if (!string.IsNullOrEmpty(nombre))  // Si hay un término de búsqueda - Filtra coincidencias
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre)); 
            }
            return query.ToList(); // Ejecuta la consulta y devuelve la lista
        }
        // Crea una nueva película en la base de datos
        public bool CrearPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            _db.Pelicula.Add(pelicula); // Agrega la entidad al contexto
            return Guardar();
        }
        // Verifica si existe una película por ID
        public bool ExistePelicula(int peliculaId)
        {
            return _db.Pelicula.Any(c => c.Id == peliculaId);  // Devuelve true si existe
        }
        // Verifica si existe una película por nombre (sin distinguir mayúsculas ni espacios)
        public bool ExistePelicula(string nombre)
        {
            bool valor = _db.Pelicula.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }
        // Obtiene una película específica por su ID
        public Pelicula GetPelicula(int peliculaId)
        {
            return _db.Pelicula.FirstOrDefault(c => c.Id == peliculaId);// Retorna la primera coincidencia o null
        }
        // Obtiene todas las películas ordenadas por nombre
        public ICollection<Pelicula> GetPeliculas()
        {
            return _db.Pelicula.OrderBy(c => c.Nombre).ToList();
        }
        // Obtiene todas las películas pertenecientes a una categoría específica
        public ICollection<Pelicula> GetPeliculasEnCategoria(int catId)
        {
            // Incluye la relación con Categoría
            // Filtra por ID de categoría
            return _db.Pelicula.Include(ca => ca.Categoria).Where(ca => ca.categoriaId == catId).ToList();
        }
        // Guarda los cambios realizados en el contexto
        public bool Guardar()
        {
            return _db.SaveChanges() > 0 ? true : false; // Devuelve true si se afectó al menos una fila
        }
    }
}
