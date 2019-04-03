using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApiPais.Models
{
    public class Provincia
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [StringLength(30, ErrorMessage = "El nombre de la provincia debe contener menos de 30 caracteres.")]
        public string Nombre { get; set; }

        /// <summary>
        /// propiedad para establecer relacion con la tabla paises
        /// </summary>
        [ForeignKey("Pais")]
        //[Key]
        //[Column(Order = 1)]
        public int PaisId { set; get; }

        /// <summary>
        /// cada provincia le corresponde un pais.
        /// </summary>
        [JsonIgnore]
        public Pais Pais { set; get; }
    }
}
