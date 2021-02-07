using DevEvents.API.Entidades;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEvents.API.Controllers
{
    [Route("api/categorias")]
    public class CategoriasController : ControllerBase
    {
        [HttpGet]
        public IActionResult ObterTodas()
        {

            var categotias = new List<Categoria>
            { 
                new Categoria { Descricao = ".NET"},
                new Categoria { Descricao = "Desenvolvimento mobile"},
                new Categoria { Descricao = "Machine Learning"}
            };

            return Ok(categotias);
        }
    }
}
