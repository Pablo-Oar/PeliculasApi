using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;

public class CategoriaRepositorio : ICategoriaRepositorio
{
	private readonly ApplicationDbContext _db;

    public CategoriaRepositorio(ApplicationDbContext db)
    {
        _db = db;
    }
    public bool ActualizarCategoria(Categoria categoria)
    {
        throw new NotImplementedException();
    }

    public bool BorrarCategoria(Categoria categoria)
    {
        throw new NotImplementedException();
    }

    public bool CrearCategoria(Categoria categoria)
    {
        throw new NotImplementedException();
    }

    public bool ExisteCategoria(int CategoriaId)
    {
        throw new NotImplementedException();
    }

    public bool ExisteCategoria(string nombre)
    {
        throw new NotImplementedException();
    }

    public Categoria GetCategoria(int CategoriaId)
    {
        throw new NotImplementedException();
    }

    public ICollection<Categoria> GetCategorias()
    {
        throw new NotImplementedException();
    }

    public bool Guardar(Categoria categoria)
    {
        throw new NotImplementedException();
    }
}
