using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiPais.Models
{

    public class AplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        //cuando se hereda de IdentityDbContext<ApplicationUser>, se crean las tablas para 
        //el manejo de usuarios.
        public AplicationDbContext(DbContextOptions<AplicationDbContext> opcion) : base(opcion) { }

        public DbSet<Pais> Paises { set; get; }
        public DbSet<Provincia> Provincias { set; get; }
    }
}
