using MainBit.Alias.Models;
using Orchard;
using Orchard.Caching;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Services
{
    public interface IEnumUrlSegmentValueRepository : IDependency
    {
        EnumUrlSegmentValueRecord Get(int id);
        void Create(EnumUrlSegmentValueRecord record);
        void Update(EnumUrlSegmentValueRecord record);
        void Delete(EnumUrlSegmentValueRecord record);
    }

    public class EnumUrlSegmentValueRepository : IEnumUrlSegmentValueRepository
    {
        private readonly IRepository<EnumUrlSegmentValueRecord> _repository;
        private readonly ISignals _signals;

        public EnumUrlSegmentValueRepository(IRepository<EnumUrlSegmentValueRecord> repository,
            ISignals signals)
        {
            _repository = repository;
            _signals = signals;
        }

        public EnumUrlSegmentValueRecord Get(int id)
        {
            return _repository.Get(id);
        }

        public void Create(EnumUrlSegmentValueRecord record)
        {
            _repository.Create(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

        public void Update(EnumUrlSegmentValueRecord record)
        {
            _repository.Update(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

        public void Delete(EnumUrlSegmentValueRecord record)
        {
            _repository.Delete(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

    }
}