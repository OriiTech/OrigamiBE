using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.CourseAccess
{
    public class GetCourseAccessResponse
    {
        public int AccessId { get; set; }
        public int? CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public int? LearnerId { get; set; }
        public string? LearnerName { get; set; }
        public DateTime? PurchasedAt { get; set; }
    }
}
