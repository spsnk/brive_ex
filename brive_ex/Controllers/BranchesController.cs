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
    public class BranchesController : ApiController
    {
        private brive_dbEntities db = new brive_dbEntities();

        // GET: api/Branches
        public IQueryable<Branch> GetBranches()
        {
            return db.Branches;
        }

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

        // Handle Inventory
        [Route("api/Branches/{branchId:int}/Products/{productId:int}")]
        [HttpPut]
        public IHttpActionResult PutInventory(int branchId, int productId, Inventory inventory) {
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

            if (branchId != inventory.BranchId && productId != inventory.ProductId)
            {
                db.Inventories.Add(inventory);
            }
            else
            {
                Inventory old_inventory = db.Inventories.Find(productId, branchId);
                old_inventory.BranchUnits += inventory.BranchUnits;

                if(old_inventory.BranchUnits < 0)
                {
                    return BadRequest();
                }
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