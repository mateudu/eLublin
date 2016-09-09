using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using eLublin.Web.Models.Db;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;


namespace eLublin.Web.Controllers
{
    [RoutePrefix("api/Reports")]
    public class ReportsController : ApiController
    {
        private elublinDbEntities db = new elublinDbEntities();

        // GET: api/Reports
        [HttpGet]
        public IQueryable<Report> GetReports()
        {
            return db.Reports;
        }

        // GET: api/Reports/5
        [HttpGet]
        [ResponseType(typeof(Report))]
        public async Task<IHttpActionResult> GetReport(int id)
        {
            Report report = await db.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        // PUT: api/Reports/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReport(int id, eLublin.Web.Models.Api.EditReport editReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (id != report.id)
            //{
            //    return BadRequest();
            //}

            decimal tempDec = 0;
            if (!decimal.TryParse(editReport.lat, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo, out tempDec) ||
                !decimal.TryParse(editReport.lng, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo, out tempDec))
            {
                return BadRequest("Zle koordynaty");
            }

            //db.Entry(report).State = EntityState.Modified;
            var report = await db.Reports.FirstAsync(x => x.id == id);
            report.lat = editReport.lat;
            report.lng = editReport.lng;
            report.tekst = editReport.tekst;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
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

        // POST: api/Reports
        [HttpPost]
        [Authorize]
        [ResponseType(typeof(Report))]
        public async Task<IHttpActionResult> PostReport(eLublin.Web.Models.Api.CommitReport commitReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            decimal tempDec = 0;
            if (!decimal.TryParse(commitReport.lat, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo, out tempDec) ||
                !decimal.TryParse(commitReport.lng, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo, out tempDec))
            {
                return BadRequest("zle koordynaty");
            }
            else
            {
                //return BadRequest("cos zjebane"+ decimal.Parse(commitReport.lat));
            }

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("elublin");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".jpg");

            // Create or overwrite the "myblob" blob with contents from a local file.
            await blockBlob.UploadFromByteArrayAsync(commitReport.image, 0, commitReport.image.Length);

            var userEmail = User.Identity.GetUserName();
            var _userInfo = await db.UserInfoes.FirstAsync(x => x.email == userEmail);

            var report = new Report {
                glosy = 0,
                imagePath = blockBlob.StorageUri.PrimaryUri.ToString(),
                lat = commitReport.lat,
                lng = commitReport.lng,
                tekst = commitReport.tekst,
                userInfoId = _userInfo.id,
                timeAdded = DateTime.Now
            };

            //FileStream fs = new FileStream(@"C:\Users\Dom\Desktop\dupa.jpg", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //fs.Write(commitReport.image, 0, commitReport.image.Length);
            //fs.Close();
            db.Reports.Add(report);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReportExists(report.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = report.id }, report);
        }

        // DELETE: api/Reports/5
        [ResponseType(typeof(Report))]
        public async Task<IHttpActionResult> DeleteReport(int id)
        {
            Report report = await db.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            db.Reports.Remove(report);
            await db.SaveChangesAsync();

            return Ok(report);
        }

        [ResponseType(typeof(void))]
        [HttpGet]
        [Route("Vote/{id}/{v}")]
        public async Task<IHttpActionResult> Vote(int id, int v)
        {
            var el = await db.Reports.FirstAsync(x => x.id == id);
            if (el==null)
            {
                return BadRequest("Zle id");
            }
            if (v == 0)
            {
                el.glosy--;
                await db.SaveChangesAsync();
                return Ok();
            }
            if (v == 1)
            {
                el.glosy++;
                el.UserInfo.exp++;
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Zla wartosc glosu - Wprowadz 0 lub 1");
        }

        [HttpGet]
        [ResponseType(typeof(IQueryable<Report>))]
        public async Task<IHttpActionResult> ReportsGPS(string lat, string lng)
        {
            decimal dLat, dLng;
            if (
                !decimal.TryParse(lat, NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture,
                    out dLat) ||
                !decimal.TryParse(lng, NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture,
                    out dLng))
            {
                return BadRequest("Zle koordynaty");
            }
            var reportsVM = db.Reports.OrderBy(x => (decimal.Parse(x.lat) - dLat) * (decimal.Parse(x.lat) - dLat)
            + (decimal.Parse(x.lng) - dLng) * (decimal.Parse(x.lng) - dLng));
            return Ok(reportsVM);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReportExists(int id)
        {
            return db.Reports.Count(e => e.id == id) > 0;
        }
    }
}