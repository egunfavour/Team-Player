using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class ProcessDto
    {
        [Required]
        public string Position { get; set; }
        [Required]
        public string MainSkill { get; set; }
        [Required]
        public int NumberOfPlayers { get; set; }
    }
}
