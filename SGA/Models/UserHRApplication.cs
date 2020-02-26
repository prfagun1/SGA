using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class UserHRApplication
    {
        public UserHRApplication()
        {
        }

        public UserHRApplication(int userHRId, int applicationId)
        {
            UserHRId = userHRId;
            ApplicationId = applicationId;
        }

        public int UserHRId { get; set; }
        public int ApplicationId { get; set; }

        public virtual Application Application { get; set; }

        public virtual UserHR UserHR { get; set; }
    }
}
