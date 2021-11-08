using System;
using System.Collections.Generic;
using HellTwitchVipApp.Models;
using HellTwitchVipApp.Models.Dto;

namespace HellTwitchVipApp.Data.Repositories
{
    public interface IGiverRepository
    {
        IEnumerable<GiverDto> GetAll();
        GiverDto GetById(Guid id);
        GiverDto GetByName(string name);
        TransactionResult<GiverDto> Add(GiverDto giver);
        TransactionResult<GiverDto> Update(GiverDto giver);
        TransactionResult<GiverDto> DeleteById(Guid id);
    }
}
