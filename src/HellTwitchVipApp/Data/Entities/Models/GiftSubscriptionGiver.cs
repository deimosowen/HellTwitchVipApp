using System;
using System.Collections.Generic;

#nullable disable

namespace HellTwitchVipApp.Data.Entities.Models
{
    public partial class GiftSubscriptionGiver
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public int? GiftCount { get; set; }
        public bool IsVip { get; set; }
        public int SortingNumber { get; set; }
    }
}
