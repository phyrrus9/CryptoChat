using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Users : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ClientModels.Users.User), StatusCodes.Status201Created)]
        public IActionResult CreateUser([FromBody, Required] ClientModels.Users.CreateModel data)
        {
            static string genid()
            {
                Random r = new();
                return $"{r.Next(0, 9)}{r.Next(0, 9)}{r.Next(0, 9)}-" +
                       $"{r.Next(0, 9)}{r.Next(0, 9)}{r.Next(0, 9)}-" +
                       $"{r.Next(0, 9)}{r.Next(0, 9)}{r.Next(0, 9)}";
            }
            using DataContext db = new();
            string Id;
            do { Id = genid(); } while (db.Users.Any(x => x.Id == Id));
            RsaKeyParameters pubkey = (RsaKeyParameters)CryptoService.ImportKey(data.ExchangePem);
            Models.User user = new()
            {
                Id = Id,
                Created = DateTime.UtcNow,
                ExchangePem = data.ExchangePem,
                PreferredName = data.DisplayName,
                ExchangeFingerprint = CryptoService.Fingerprint(pubkey)
            };
            db.Users.Add(user);
            db.SaveChanges();
            return StatusCode((int)HttpStatusCode.Created, user);
        }

        [HttpGet, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ClientModels.Users.User), StatusCodes.Status200OK)]
        public IActionResult GetUser(string Id)
        {
            using DataContext db = new();
            if (db.Users.Find(Id) is Models.User user && user != null)
                return Ok(user);
            return NotFound();
        }

        [HttpPatch, Route("{id}")]
        [ProducesResponseType(typeof(ClientModels.Users.User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult UpdateUser(string Id, [FromBody, Required]ClientModels.Users.EditModel mdl)
        {
            if (Id != mdl.GetUserId() || !mdl.VerifyData())
                return Unauthorized();
            using DataContext db = new();
            Models.User user = db.Users.Find(Id);
            user.Merge(mdl);
            db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
            return Ok(user);
        }

        [HttpGet, Route("Me")]
        [ProducesResponseType(typeof(ClientModels.Users.User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me([FromBody, Required]ClientModels.SignedMessage signature)
        {
            string Id = signature.GetUserId();
            if (!signature.VerifyData())
                return Unauthorized();
            using DataContext db = new();
            return Ok(db.Users.Find(Id));
        }
    }
}
