﻿using MainBit.Alias.Models;
using Orchard;
using Orchard.Caching;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Services
{
    public interface IEnumUrlSegmentService : IDependency
    {
        List<EnumUrlSegmentRecord> GetList();
        EnumUrlSegmentRecord Get(int id);
        void Create(EnumUrlSegmentRecord record);
        void Update(EnumUrlSegmentRecord record);
        void Delete(EnumUrlSegmentRecord record);
    }

    public class EnumUrlSegmentService : IEnumUrlSegmentService
    {
        private readonly IRepository<EnumUrlSegmentRecord> _repository;
        private readonly ISignals _signals;

        public EnumUrlSegmentService(IRepository<EnumUrlSegmentRecord> repository,
            ISignals signals)
        {
            _repository = repository;
            _signals = signals;
        }

        public List<EnumUrlSegmentRecord> GetList()
        {
            return _repository.Table.OrderBy(p => p.Position).ToList();
        }

        public EnumUrlSegmentRecord Get(int id)
        {
            return _repository.Get(id);
        }

        public void Create(EnumUrlSegmentRecord record)
        {
            _repository.Create(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

        public void Update(EnumUrlSegmentRecord record)
        {
            _repository.Update(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

        public void Delete(EnumUrlSegmentRecord record)
        {
            _repository.Delete(record);
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
        }

    }
}