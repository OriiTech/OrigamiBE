using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{
    public class ChallengeListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

        public string Status { get; set; }
        public string Phase { get; set; }

        public string PromoPhoto { get; set; }
        public string Theme { get; set; }
        public string Level { get; set; }

        public List<string> Category { get; set; }

        public OrganizerDto Organizer { get; set; }
        public ChallengeStatsDto Stats { get; set; }

        public bool? IsParticipating { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsFree { get; set; }
        public decimal? EntryFee { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ChallengeStatsDto
    {
        public int ParticipantsCount { get; set; }
        public int SubmissionsCount { get; set; }
        public decimal? PrizePool { get; set; }
    }

    public class OrganizerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
    }
}
