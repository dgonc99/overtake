using Microsoft.Extensions.ObjectPool;

namespace Overtake.Models
{
    public class RaceLeagueInfo
    {
        public required int OwnerId { get; set; }
        public required string Name { get; set; }
        public required Boolean IsPublic { get; set; }
    }
}
