using AutoMapper;
using HellTwitchVipApp.Data.Concrete;
using HellTwitchVipApp.Data.Entities.Models;
using HellTwitchVipApp.Models;
using HellTwitchVipApp.Models.Dto;
using HellTwitchVipApp.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HellTwitchVipApp.Data.Repositories
{
    public sealed class GiverRepository: IGiverRepository, IDisposable
    {
        private readonly HellAppContext _context;
        private readonly IMapper _mapper;
        private bool _disposed;

        public GiverRepository(HellAppContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<GiverDto> GetAll()
        {
            return _mapper.Map<IEnumerable<GiverDto>>(_context.GiftSubscriptionGivers);
        }

        public GiverDto GetById(Guid id)
        {
            return _mapper.Map<GiverDto>(_context.GiftSubscriptionGivers.Find(id));
        }

        public GiverDto GetByName(string name)
        {
            return _mapper.Map<GiverDto>(_context.GiftSubscriptionGivers.SingleOrDefault(s => s.UserName.ToLower() == name.ToLower().Trim()));
        }

        public TransactionResult<GiverDto> Add(GiverDto giver)
        {
            if (giver is null)
                throw new ArgumentNullException(nameof(giver));

            var result = new TransactionResult<GiverDto>(giver);
            try
            {
                giver.Id = Guid.NewGuid();
                var subscriptionGiver = _mapper.Map<GiverDto, GiftSubscriptionGiver>(giver);
                _context.Add(subscriptionGiver);
                _context.SaveChanges();
                return result;
            }
            catch (Exception e)
            {
                result.Error(e.Message);
            }

            return result;
        }

        public TransactionResult<GiverDto> Update(GiverDto giver)
        {
            if (giver is null)
                throw new ArgumentNullException(nameof(giver));

            var result = new TransactionResult<GiverDto>(giver);
            try
            {
                var giftSubscriptionGiver = _context.GiftSubscriptionGivers.SingleOrDefault(m => m.Id == giver.Id);
                var subscriptionGiver = _mapper.Map<GiverDto, GiftSubscriptionGiver>(giver, giftSubscriptionGiver);
                _context.GiftSubscriptionGivers.AsNoTracking();
                _context.Entry(subscriptionGiver).State = EntityState.Modified;
                _context.SaveChanges();
                return result;
            }
            catch (Exception e)
            {
                result.Error(e.Message);
            }

            return result;
        }

        public TransactionResult<GiverDto> DeleteById(Guid id)
        {
            var result = new TransactionResult<GiverDto>(null);
            try
            {
                var giftSubscriptionGiver = _context.GiftSubscriptionGivers.Find(id);
                _context.Remove(giftSubscriptionGiver);
                _context.SaveChanges();
                return result;
            }
            catch (Exception e)
            {
                result.Error(e.Message);
            }

            return result;
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}