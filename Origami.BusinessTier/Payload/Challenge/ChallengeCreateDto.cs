using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{
    public class ChallengeCreateDto
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string PromoPhoto { get; set; }

        public string Theme { get; set; }
        public string Level { get; set; }

        public List<string> Category { get; set; } = new();

        public int OrganizerId { get; set; }

        public decimal PrizePool { get; set; }
        public bool IsFree { get; set; }
        public decimal? EntryFee { get; set; }

        // Requirements
        public string PaperRequirements { get; set; }
        public string FoldingConstraints { get; set; }
        public string PhotographyRequirements { get; set; }
        public string ModelRequirements { get; set; }
        public List<string> OtherRequirements { get; set; } = new();

        public int? MaximumSubmissions { get; set; }
        public int? TeamSize { get; set; }

        // Schedule
        public DateTime? RegistrationStart { get; set; }
        public DateTime? SubmissionStart { get; set; }
        public DateTime? SubmissionEnd { get; set; }
        public DateTime? VotingStart { get; set; }
        public DateTime? VotingEnd { get; set; }
        public DateTime? ResultsDate { get; set; }

        public List<ChallengeRuleCreateDto> Rules { get; set; } = new();
    }
    public class ChallengeRuleCreateDto
    {
        public string Section { get; set; }
        public string Items { get; set; } 
    }

}
