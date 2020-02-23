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
    /// Controlador de Productos.
    /// </summary>
    public class ProductsController : ApiController
    {
        private readonly brive_dbEntities db = new brive_dbEntities();

        /// <summary>
        /// Hace una consulta de todos los productos registrados.
        /// </summary>
        /// <returns>Lista de Objetos &lt;Product&gt; IQueryable&lt;Product&gt;</returns>
        // GET: api/Products
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        /// <summary>
        /// Hace una consulta de producto, dado un ID.
        /// </summary>
        /// <param name="id">ID de Producto a consultar.</param>
        /// <returns>Objeto &lt;Product&gt; consultado.</returns>
        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Modifica la Información de un producto.
        /// </summary>
        /// <param name="id">ID de producto a modificar.</param>
        /// <param name="product">Objeto &lt;Product&gt;</param>
        /// <returns>&lt;void&gt;</returns>
        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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
        /// Registra un nuevo producto.
        /// </summary>
        /// <param name="product">Objeto &lt;Product&gt;</param>
        /// <returns>Objeto &lt;Product&gt; eliminado.</returns>
        // POST: api/Products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
        }

        /// <summary>
        /// Elimina un Producto, dado un ID.
        /// </summary>
        /// <param name="id">ID del producto a eliminar.</param>
        /// <returns>&lt;Product&gt;</returns>
        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        /// <summary>
        /// Hace una busqueda parcial con base en el nombre del producto.
        /// </summary>
        /// <param name="term">Termino a buscar en el nombre de los productos.</param>
        /// <returns>Lista de Objetos &lt;Product&gt; IQueryable&lt;Product&gt;</returns>
        [Route("api/Products/Search/{term}")]
        [HttpGet]
        public IQueryable<Product> SearchProduct(string term)
        {
            List<Product> res = db.Products.Where(x => x.ProductName.Contains(term)).ToList();
            return res.AsQueryable();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}