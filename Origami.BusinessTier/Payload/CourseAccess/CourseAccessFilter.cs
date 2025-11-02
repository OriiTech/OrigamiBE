using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.CourseAccess
{
    public class CourseAccessFilter
    {
        public int? CourseId { get; set; }
        public int? LearnerId { get; set; }
    }
}