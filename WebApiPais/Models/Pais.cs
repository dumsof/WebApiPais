using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPais.Models
{
    public class Pais
    {
        //para que la lista de provincia no se presente en null, para que muestre un arreglo vacio.
        public Pais()
        {
            Provincias = new List<Provincia>();
        }
        public int Id { get; set; }

        [StringLength(30, ErrorMessage = "El nombre del pais debe contener menos de 30 caracteres.")]
        public string Nombre { get; set; }

        /// <summary>
        /// cada pais le corresponde un listado de provincias, para realizar la 
        /// relación con la tabla de provincias.
        /// </summary>
        public List<Provincia> Provincias { set; get; }
    }
}
