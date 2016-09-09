using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using eLublin.Web.Models.Db;

namespace eLublin.Web.Controllers
{
    [RoutePrefix("api/Notifications")]
    public class NotificationsController : ApiController
    {
        private elublinDbEntities db = new elublinDbEntities();

        // GET: api/Notifications
        public IQueryable<Notification> GetNotifications()
        {
            var list = db.Notifications;
            list.OrderBy(x => x.id);
            return list;
        }

        // GET: api/Notifications/5
        [ResponseType(typeof(Notification))]
        public async Task<IHttpActionResult> GetNotification(int id)
        {
            Notification notification = await db.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        // PUT: api/Notifications/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNotification(int id, eLublin.Web.Models.Api.CommitNotification commitNotification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //db.Entry(notification).State = EntityState.Modified;
            Notification notification = await db.Notifications.FirstAsync(x => x.id == id);
            notification.tekst = commitNotification.tekst;
            notification.url = commitNotification.url;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
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

        // POST: api/Notifications
        [ResponseType(typeof(Notification))]
        public async Task<IHttpActionResult> PostNotification(eLublin.Web.Models.Api.CommitNotification commitNotification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Notification notification = new Notification { czasDodania = DateTime.Now, tekst = commitNotification.tekst, url = commitNotification.url, tytul = commitNotification.tytul};

            db.Notifications.Add(notification);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NotificationExists(notification.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = notification.id }, notification);
        }

        // DELETE: api/Notifications/5
        [ResponseType(typeof(Notification))]
        public async Task<IHttpActionResult> DeleteNotification(int id)
        {
            Notification notification = await db.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            db.Notifications.Remove(notification);
            await db.SaveChangesAsync();

            return Ok(notification);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NotificationExists(int id)
        {
            return db.Notifications.Count(e => e.id == id) > 0;
        }
    }
}