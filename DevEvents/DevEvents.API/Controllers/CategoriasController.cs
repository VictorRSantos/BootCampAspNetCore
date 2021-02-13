using Dapper;
using DevEvents.API.Entidades;
using DevEvents.API.Persistencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEvents.API.Controllers
{
    [Route("api/categorias")]
    public class CategoriasController : ControllerBase
    {
        private readonly DevEventsDbContext _dbContext;
        public CategoriasController(DevEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult ObterTodas()
        {
            #region Dapper
            //var connectionString = _dbContex.Database.GetDbConnection().ConnectionString;

            //using (var sqlConnection = new SqlConnection(connectionString))
            //{

            //    var script = "SELECT Id, Descricao FROM Categorias";

            //    return Ok(sqlConnection.Query<Categoria>(script));


            //}

            #endregion

            #region EntityFramework
            var categoria = _dbContext.Categorias.ToList();

            return Ok(categoria);
            #endregion
        }
    }
}
