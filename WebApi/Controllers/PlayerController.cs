// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly DataContext Context;

    public PlayerController(DataContext context)
    {
        Context = context;
    }

  [HttpGet]
  public  async Task<ActionResult<IEnumerable<Player>>> GetAll()
  {
        try
        {
           var result = await Context.Players.Include(p => p.PlayerSkills).ToListAsync();
           return Ok(result);
        }catch(Exception ex)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, ex);
        }
  }

  [HttpPost]
  public async Task<ActionResult<Player>> PostPlayer(Player player)
  {
        try
        {
            await Context.Players.AddAsync(player);
            await Context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.Created, player);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, ex);
        }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> PutPlayer(int id, Player player)
  {
        try
        {
            Player playerUpdate = await Context.Players.Include(p => p.PlayerSkills)
                  .Where(p => p.Id == id).FirstOrDefaultAsync();

            if (playerUpdate != null)
            {
                playerUpdate.Name = player.Name;
                playerUpdate.Position = player.Position;
                foreach (var playerSkill in player.PlayerSkills)
                {
                    foreach(var currentSkill in playerUpdate.PlayerSkills)
                    {
                        currentSkill.Skill = playerSkill.Skill;
                        currentSkill.Value = playerSkill.Value;
                    }

                    Context.Attach(playerUpdate);
                    Context.Entry(playerUpdate).State = EntityState.Modified;

                    Context.SaveChanges();
                    return StatusCode((int)HttpStatusCode.OK, playerUpdate);
                }
            }

            return StatusCode((int)HttpStatusCode.NotFound, new ErrorResponse($"Player Not Found"));
        }
        catch(Exception ex)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, ex);
        }
  }

  
  [HttpDelete("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<Player>> DeletePlayer(int id)
    {
        try
        {
            var _DeletePlayer = (from a in Context.Players where a.Id == id select a).FirstOrDefault(); 
            if (_DeletePlayer != null)
            {
                Context.Players.Remove(_DeletePlayer);
                await Context.SaveChangesAsync();
                return StatusCode((int)HttpStatusCode.OK, id);
            }
            else
            { 
                return StatusCode((int)HttpStatusCode.NotFound, new ErrorResponse($"Member not found! {id}"));
            }
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, ex);
        }
    }
}