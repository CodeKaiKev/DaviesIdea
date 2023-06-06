using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DaviesIdeas.Data;
using DaviesIdeas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DaviesIdeas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdeasController : ControllerBase
    {
        private readonly DataContext _context;

        public IdeasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Ideas get all ideas
        [HttpGet, Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<Idea>>> GetIdeas()
        {
            try {
                var userId = HttpContext.Session.GetInt32(SessionVariables.SessionKeyId);
                var ideaUser = await _context.Users.FindAsync(userId);
                if (ideaUser.Role == "Admin")
                {
                    var users = await _context.Users.ToListAsync();
                }

                return Ok(_context.Ideas);
            }
            catch (Exception e) { 
                return BadRequest(e.Message);
            }
            
        }

        // GET: api/Ideas/5 get single idea
        [HttpGet("{id}"), Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<Idea>> GetIdea(int id)
        {
            try
            {
                var idea = await _context.Ideas.FindAsync(id);
                //check admin or user
                var userId = HttpContext.Session.GetInt32(SessionVariables.SessionKeyId);
                var ideaUser = await _context.Users.FindAsync(userId);

                if (idea == null)
                {
                    return BadRequest("Item not found");
                }

                if (ideaUser.Role == "User")
                {
                    if (userId != idea.UserId)
                    {
                        return BadRequest("As a user you can not view in detail an item that is not yours (Admin privilleges).");
                    }
                }
                if(ideaUser.Role == "Admin")
                {
                    var users = await _context.Users.ToListAsync();
                }

                
                return Ok(idea);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        // PUT: api/Ideas/5 update idea Updating an admin can update everything a user can update only required.

        [HttpPut("{id}"), Authorize(Roles ="Admin,User")]
        public async Task<IActionResult> PutIdea(int id, CreateIdea Cidea)
        {
            try
            {

                var idea = await _context.Ideas.FindAsync(id);
                if (idea == null)
                {
                    return BadRequest("Item not found");
                }
                //check admin or user
                var userId = HttpContext.Session.GetInt32(SessionVariables.SessionKeyId);
                var ideaUser = await _context.Users.FindAsync(userId);

                if (ideaUser.Role == "User")
                {
                    if (userId != idea.UserId)
                    {
                        return BadRequest("As a user you can not edit an item that is not yours (Admin privilleges).");
                    }
                }
                if (ideaUser.Role == "Admin")
                {
                    var users = await _context.Users.ToListAsync();
                }


                if (Cidea.Title != null)
                    idea.Title = Cidea.Title;
                if (Cidea.Description != null)
                    idea.Description = Cidea.Description;

                await _context.SaveChangesAsync();

                return Ok(new { _context.Ideas, idea, ideaUser });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            

        }

        // POST: api/Ideas

        [HttpPost, Authorize(Roles="Admin,User")]
        public async Task<ActionResult<Idea>> PostIdea(CreateIdea ideaReq)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32(SessionVariables.SessionKeyId);
                var ideaUser = await _context.Users.FindAsync(userId);
                if (ideaUser.Role == "Admin")
                {
                    var users = await _context.Users.ToListAsync();
                }

                HttpContext.Session.GetString(SessionVariables.SessionKeyUsername);
                var registeredUser = (from c in _context.Users
                                      where c.Username.Equals(HttpContext.Session.GetString(SessionVariables.SessionKeyUsername))
                                      select c).SingleOrDefault();

                Idea idea = new Idea();
                idea.Title = ideaReq.Title;
                idea.Description = ideaReq.Description;

                if (registeredUser != null)
                {
                    idea.User = registeredUser;
                    idea.UserId = registeredUser.Id;

                }

                _context.Ideas.Add(idea);
                await _context.SaveChangesAsync();
                return Ok(_context.Ideas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

           
        }

        // DELETE: api/Ideas/5
        [HttpDelete("{id}"), Authorize(Roles="Admin,User")]
        public async Task<IActionResult> DeleteIdea(int id)
        {
            try
            {
                var idea = await _context.Ideas.FindAsync(id);
                if (idea == null)
                {
                    return NotFound();
                }
                //Admin vs User
                var userId = HttpContext.Session.GetInt32(SessionVariables.SessionKeyId);
                var ideaUser = await _context.Users.FindAsync(userId);
                if (ideaUser.Role == "User")
                {
                    if (userId != idea.UserId)
                    {
                        return BadRequest("As a user you can not delete an item that is not yours (Admin privilleges).");
                    }
                }
                if (ideaUser.Role == "Admin")
                {
                    var users = await _context.Users.ToListAsync();
                }

                _context.Ideas.Remove(idea);
                await _context.SaveChangesAsync();

                return Ok(_context.Ideas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

           
        }
    }
}
