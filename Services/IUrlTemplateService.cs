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
    public interface IUrlTemplateService : IDependency
    {
        List<UrlTemplateRecord> GetList();
        UrlTemplateRecord Get(int id);
        void Create(UrlTemplateRecord record);
        void Update(UrlTemplateRecord record);
        void Delete(UrlTemplateRecord record);
    }

    public class BaseUrService : IUrlTemplateService
    {
        private readonly IRepository<UrlTemplateRecord> _repository;
        private readonly ISignals _signals;

        public BaseUrService(IRepository<UrlTemplateRecord> repository,
            ISignals signals)
        {
            _repository = repository;
            _signals = signals;
        }

        public List<UrlTemplateRecord> GetList()
        {
            return _repository.Table.OrderBy(p => p.Position).ToList();
        }

        public UrlTemplateRecord Get(int id)
        {
            return _repository.Get(id);
        }

        public void Create(UrlTemplateRecord record)
        {
            _repository.Create(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

        public void Update(UrlTemplateRecord record)
        {
            _repository.Update(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

        public void Delete(UrlTemplateRecord record)
        {
            _repository.Delete(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

    }
}