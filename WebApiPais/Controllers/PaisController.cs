using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPais.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApiPais.Controllers
{
    [Produces("application/json")]
    [Route("api/Pais")]
    //definir el esquema de autorizacion, en este caso json web token
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class PaisController : Controller
    {
        private readonly AplicationDbContext context;
        public PaisController(AplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IEnumerable<Pais> Get()
        {
            return context.Paises.ToList();
        }

        [HttpGet("{id}", Name = "PaisCreado")]
        public IActionResult GetById(int id)
        {
            var pais = context.Paises.Include(p => p.Provincias).FirstOrDefault(c => c.Id == id);
            if (pais == null)
                return NotFound();
            return Ok(pais);
        }


        [HttpPost]
        public IActionResult Post([FromBody] Pais pais)
        {
            if (ModelState.IsValid)
            {
                context.Paises.Add(pais);
                context.SaveChanges();
                //redirecciona a la acion GetById con el verbo PaisCreado, pasando como parametro el id del nuevo pais creado.
                return new CreatedAtRouteResult("PaisCreado", new { id = pais.Id }, pais);
            }
            //permite llevar los mensajes de validacion definidos en el modelo
            //hasta la aplicacion cliente cuando trata de guardar un nombre que
            //pasa de los 30 caracteres para el pais           
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public IActionResult Post([FromBody] Pais pais, int id)
        {
            if (pais.Id != id)
            {
                return BadRequest();
            }
            context.Entry(pais).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var pais = context.Paises.FirstOrDefault(c => c.Id == id);
            if (pais == null)
            {
                return NotFound();
            }
            context.Paises.Remove(pais);
            context.SaveChanges();
            context.Paises.Remove(pais);
            return Ok();
        }
    }
}