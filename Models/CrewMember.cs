using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Models
{
    public class CrewMember
    {
        public int Id { get; set; }   

        [Required, StringLength(100)] 
        public string Name { get; set; }



    }
}
