using HellTwitchVipApp.Models.Dto;
using System;

namespace HellTwitchVipApp.Models
{
    public static class GiverExtension
    {
        public static int GetVipCount(this GiverDto giver)
        {
            return Math.DivRem(giver.Count, 15, out var result);
        }
    }
}
