using HellTwitchVipApp.Models.Dto;
using System.Collections.Generic;
using System.Linq;

namespace HellTwitchVipApp.Models.Response
{
    public sealed class GiversResponse
    {
        public GiversResponse(IEnumerable<GiverDto> givers)
        {
            AllGivers = givers.OrderByDescending(o => o.Count).ThenBy(y => y.Name).ToList();
            VipGivers = AllGivers.Where(w => w.IsVip);
            WithoutVipGivers = AllGivers.Where(w => !w.IsVip);
        }
        private IEnumerable<GiverDto> AllGivers { get; }
        public IEnumerable<GiverDto> VipGivers { get; }
        public IEnumerable<GiverDto> WithoutVipGivers { get; }
        public bool IsAdmin { get; private set; }
        public void Admin() => IsAdmin = true;
    }
}
