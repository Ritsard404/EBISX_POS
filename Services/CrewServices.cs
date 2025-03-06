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
                new CrewMember { Id = 101, Name = "John Doe" },
                new CrewMember { Id = 102, Name = "Jane Smith" },
                new CrewMember { Id = 103, Name = "Robert Brown" },
                new CrewMember { Id = 104, Name = "Emily Davis" },
                new CrewMember { Id = 105, Name = "Michael Johnson" }
            };
        }

        public List<CrewMember> GetAllCrewMembers() => _crewMembers;

        public CrewMember? Authenticate(int crewId)
        {
            return _crewMembers.FirstOrDefault(c => c.Id == crewId);
        }
    }
}
