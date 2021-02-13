using Dapper;
using DevEvents.API.Entidades;
using DevEvents.API.Persistencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace DevEvents.API.Controllers
{
    [Route("api/eventos")]
    public class EventosController : ControllerBase
    {
        private readonly DevEventsDbContext _dbContext;
        public EventosController(DevEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public IActionResult ObterEventos()
        {
            var eventos = _dbContext.Eventos.ToList();

            return Ok(eventos);

        }

        // api/eventos/1
        [HttpGet("{id}")]
        public IActionResult ObterEvento(int id)
        {
            var evento = _dbContext
                .Eventos
                .Include(e => e.Categoria)
                .Include(e => e.Usuario)
                .Include(e => e.Inscricoes)
                .SingleOrDefault(e => e.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            return Ok(evento);
        }


        // api/eventos
        [HttpPost]
        public IActionResult Cadastrar([FromBody] Evento evento)
        {
            if (evento != null)
            {

                _dbContext.Eventos.Add(evento);
                _dbContext.SaveChanges();
            }
            return NoContent();
        }


        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Evento evento)
        {
            if (evento != null)
            {
                _dbContext.Eventos.Update(evento);
                _dbContext.Entry(evento).Property(e => e.DataCadastro).IsModified = false;
                _dbContext.Entry(evento).Property(e => e.Ativo).IsModified = false;
                _dbContext.Entry(evento).Property(e => e.IdUsuario).IsModified = false;

                _dbContext.SaveChanges();

            }

            return NoContent();
        }

        // api/eventos/1
        [HttpDelete("{id}")]
        public IActionResult Cancelar(int id)
        {
            #region Entity Framework
            //var evento = _dbContext.Eventos.SingleOrDefault(e => e.Id == id);

            //if (evento == null)
            //{
            //    return NotFound();
            //}

            //evento.Ativo = false;

            //_dbContext.SaveChanges();
            #endregion

            #region Dapper
            var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;

            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    var script = "UPDATE Eventos SET Ativo = 0 WHERE Id = @id";

                    sqlConnection.Execute(script, new { id });
                }

                transactionScope.Complete();
            }


            //using (var sqlConnection = new SqlConnection(connectionString))
            //{
            //    using (var transaction = sqlConnection.BeginTransaction())
            //    {
            //        try
            //        {
            //            var script = "UPDATE Eventos SET Ativo = 0 WHERE Id = @id";

            //            sqlConnection.Execute(script, new { id });

            //            transaction.Commit();
            //        }
            //        catch (System.Exception)
            //        {

            //            transaction.Rollback();

            //        }
            //    }

            //}



            #endregion

            return NoContent();
        }


        // api/evento/1/usuarios/3/inscrever
        [HttpPost("{id}/usuarios/{idUsuario}/inscrever")]
        public IActionResult Inscrever(int id, int idUsuario, [FromBody] Inscricao inscricao)
        {
            inscricao = new Inscricao();
            inscricao.IdEvento = id;
            inscricao.IdUsuario = idUsuario;


            var evento = _dbContext.Eventos.SingleOrDefault(e => e.Id == id);

            if (!evento.Ativo)
            {
                return BadRequest();
            }

            _dbContext.Inscricoes.Add(inscricao);

            _dbContext.SaveChanges();

            return NoContent();
        }


        [HttpPost("popular")]
        public IActionResult Popular()
        {
            var usuario = new Usuario
            {
                NomeCompleto = "Franco  Dev",
                Email = "franco@gmail.com"
            };

            var categorias = new List<Categoria>
            {
                new  Categoria{Descricao="C#" },
                new  Categoria{Descricao="Flutter" },
                new  Categoria{Descricao="Xamarim" },
            };

            _dbContext.Usuarios.Add(usuario);
            _dbContext.Categorias.AddRange(categorias);

            _dbContext.SaveChanges();

            return NoContent();
        }

    }
}
