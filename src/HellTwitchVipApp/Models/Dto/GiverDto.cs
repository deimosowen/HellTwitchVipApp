using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HellTwitchVipApp.Models.Dto
{
    public class GiverDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public bool IsVip { get; set; }
    }
}
