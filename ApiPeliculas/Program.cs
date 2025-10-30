using ApiPeliculas.Data;
using ApiPeliculas.PeliculasMappers;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
                    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

// Agregamos los repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var secretKey = builder.Configuration.GetValue<string>("ApiSettings:secretKey");

//Agregamos el AutoMapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper).Assembly);

//Se configura la autenticacion
builder.Services.AddAuthentication
    (
       x =>
       {
           x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
           x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
       }
    ).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }
    );


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Soporte para CORS
//Se pueden habilitar: 1-Un dominio. 2-Multiples dominios, 3- Cualquier dominio (Tener en cuenta seguridad).
//Uso de ejemplo el dominio: http://localhost:3223, se debe cambiar por el correcto.
//Se usa (*) para todos los dominios.
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.WithOrigins("http://localhost:3223").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//soporte para CORS
app.UseCors("PoliticaCors");

//Soporte para autenticacion
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
