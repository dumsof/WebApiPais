using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPais.Models;

namespace WebApiPais.Controllers
{
    [Produces("application/json")]
    //se agrega a la regla de ruteo el pais y luego la provincia para obtener la informacion por
    //pais y luego las provincias de ese pais
    [Route("api/Pais/{PaisId}/Provincia")]
    public class ProvinciaController : Controller
    {

        private readonly AplicationDbContext context;
        public ProvinciaController(AplicationDbContext context)
        {
            this.context = context;
        }

        //http://localhost:63146/api/pais/2/provincia
        [HttpGet]
        public IEnumerable<Provincia> Get(int? PaisId)
        {
            return context.Provincias.Where(c => c.PaisId == PaisId).ToList();
        }

        [HttpGet("{id}", Name = "ProvinciaCreada")]
        public IActionResult GetById(int id)
        {
            var pais = context.Provincias.FirstOrDefault(c => c.Id == id);
            if (pais == null)
                return NotFound();
            return Ok(pais);
        }


        [HttpPost]
        public IActionResult Post([FromBody] Provincia provincia)
        {
            if (ModelState.IsValid)
            {
                context.Provincias.Add(provincia);
                context.SaveChanges();
                //redirecciona a la acion GetById con el verbo PaisCreado, pasando como parametro el id del nuevo pais creado.
                return new CreatedAtRouteResult("ProvinciaCreada", new { id = provincia.Id }, provincia);
            }
            //permite llevar los mensajes de validacion definidos en el modelo
            //hasta la aplicacion cliente cuando trata de guardar un nombre que
            //pasa de los 30 caracteres para el pais           
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public IActionResult Post([FromBody] Provincia provincia, int id)
        {
            if (provincia.Id != id)
            {
                return BadRequest();
            }
            context.Entry(provincia).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Delete(int id)
        {
            var provincia = context.Provincias.FirstOrDefault(c => c.Id == id);
            if (provincia == null)
            {
                return NotFound();
            }
            context.Provincias.Remove(provincia);
            context.SaveChanges();
            context.Provincias.Remove(provincia);
            return Ok();
        }
    }
}