using EBISX_POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Services
{
    public class CrewServices
    {
        private readonly List<CrewMember> _crewMembers;

        public CrewServices()
        {
            _crewMembers = new List<CrewMember>
            {
                new CrewMember { CrewMemberId = 101, Name = "John Doe" },
                new CrewMember { CrewMemberId = 102, Name = "Jane Smith" },
                new CrewMember { CrewMemberId = 103, Name = "Robert Brown" },
                new CrewMember { CrewMemberId = 104, Name = "Emily Davis" },
                new CrewMember { CrewMemberId = 105, Name = "Michael Johnson" }
            };
        }

        public List<CrewMember> GetAllCrewMembers() => _crewMembers;

        public CrewMember? Authenticate(int crewId)
        {
            return _crewMembers.FirstOrDefault(c => c.CrewMemberId == crewId);
        }
    }
}
