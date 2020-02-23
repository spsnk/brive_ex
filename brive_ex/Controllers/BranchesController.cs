using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using brive_DataAccess;

namespace brive_ex.Controllers
{
    /// <summary>
    /// Controlador de Sucursales.
    /// </summary>
    public class BranchesController : ApiController
    {
        private readonly brive_dbEntities db = new brive_dbEntities();

        /// <summary>
        /// Hace una consulta de todas las sucursales registradas.
        /// </summary>
        /// <returns>Lista de Objetos &lt;Branch&gt; IQueryable&lt;Branch&gt;</returns>
        // GET: api/Branches
        public IQueryable<Branch> GetBranches()
        {
            return db.Branches;
        }

        /// <summary>
        /// Hace una consulta de sucursal, dado un ID.
        /// </summary>
        /// <param name="id">ID de sucursal a consultar.</param>
        /// <returns>Objeto &lt;Branch&gt;</returns>
        // GET: api/Branches/5
        [ResponseType(typeof(Branch))]
        public IHttpActionResult GetBranch(int id)
        {
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return NotFound();
            }

            return Ok(branch);
        }

        /// <summary>
        /// Modifica los datos de una sucursal, dado un ID.
        /// </summary>
        /// <param name="id">ID de la sucursal a modificar.</param>
        /// <param name="branch">Objeto &lt;Branch&gt;</param>
        /// <returns>&lt;void&gt;</returns>
        // PUT: api/Branches/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBranch(int id, Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != branch.BranchId)
            {
                return BadRequest();
            }

            db.Entry(branch).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Registra una nueva sucursal.
        /// </summary>
        /// <param name="branch">Objeto &lt;Branch&gt;</param>
        /// <returns>Objeto &lt;Branch&gt; registrado.</returns>
        // POST: api/Branches
        [ResponseType(typeof(Branch))]
        public IHttpActionResult PostBranch(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Branches.Add(branch);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = branch.BranchId }, branch);
        }

        /// <summary>
        /// Elimina una sucursal, dado un ID.
        /// </summary>
        /// <param name="id">ID de la sucursal a eliminar.</param>
        /// <returns>Objeto &lt;Branch&gt; eliminado.</returns>
        // DELETE: api/Branches/5
        [ResponseType(typeof(Branch))]
        public IHttpActionResult DeleteBranch(int id)
        {
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return NotFound();
            }

            db.Branches.Remove(branch);
            db.SaveChanges();

            return Ok(branch);
        }

        /// <summary>
        /// Modifica la información del inventario de una sucursal, para un producto dado.
        /// </summary>
        /// <param name="branchId">ID de la sucursal a modificar.</param>
        /// <param name="productId">ID del producto a modificar.</param>
        /// <param name="units">Cantidad de unidades a agregar/remover.</param>
        /// <returns>&lt;void&gt;</returns>
        // Handle Inventory
        [Route("api/Branches/{branchId:int}/Products/{productId:int}/")]
        [HttpPut]
        public IHttpActionResult PutInventory(int branchId, int productId, int units) {
            Branch branch = db.Branches.Find(branchId);
            Product product = db.Products.Find(productId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (branch == null || product == null)
            {
                return BadRequest();
            }
            
            Inventory inventory = db.Inventories.Find(productId, branchId);
            inventory.BranchUnits += units;

            if(inventory.BranchUnits < 0)
            {
                return BadRequest();
            }
            
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(branchId) || !(db.Products.Count(e => e.ProductId == productId) > 0) )
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BranchExists(int id)
        {
            return db.Branches.Count(e => e.BranchId == id) > 0;
        }
    }
}