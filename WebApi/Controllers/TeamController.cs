// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly DataContext Context;

        public TeamController(DataContext context)
        {
            Context = context;
        }

        [HttpPost("process")]
        public async Task<ActionResult<List<Player>>> Process([FromBody] List<ProcessDto> processes)
        {
            var players = await Context.Players.Include(p => p.PlayerSkills).ToListAsync();
            var selectedPlayers = new List<Player>();

            foreach(var single in processes)
            {
                if(processes.Count(s => s.Position == single.Position) > 1)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse($"The position of the player should not be repeated in the request."));
                }

                if(players.Count(p => p.Position == single.Position) < single.NumberOfPlayers)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse($"Insufficient number of players for position: {single.Position}"));
                }

                var matchingPlayers = players.Where(p => p.Position == single.Position).Where(p => p.PlayerSkills.Any(s => s.Skill == single.MainSkill)).OrderByDescending(p => p.PlayerSkills.First(s => s.Skill == single.MainSkill).Value).ToList();

                if(matchingPlayers.Count > 0 && matchingPlayers.Count >= single.NumberOfPlayers)
                {
                    selectedPlayers.AddRange(matchingPlayers.GetRange(0, single.NumberOfPlayers));
                }
                else
                {
                    matchingPlayers = players.Where(p => p.Position == single.Position).OrderByDescending(p => p.PlayerSkills.First(s => s.Value == p.PlayerSkills.Select(s => s.Value).Max()).Value).ToList();
                    if (matchingPlayers.Count > 0 && matchingPlayers.Count >= single.NumberOfPlayers)
                    {
                        selectedPlayers.AddRange(matchingPlayers.GetRange(0, single.NumberOfPlayers));
                    }
                }
            }

            return Ok(selectedPlayers);
        }
    }
}
