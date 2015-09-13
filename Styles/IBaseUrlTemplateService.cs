using MainBit.Alias.Models;
using Orchard;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Services
{
    public interface IBaseUrlTemplateService : IDependency
    {
        List<BaseUrlTemplateRecord> GetList();
        BaseUrlTemplateRecord Get(int id);
        void Create(BaseUrlTemplateRecord record);
        void Update(BaseUrlTemplateRecord record);
        void Delete(BaseUrlTemplateRecord record);
    }

    public class BaseUrlTemplateService : IBaseUrlTemplateService
    {
        private readonly IRepository<BaseUrlTemplateRecord> _repository;

        public BaseUrlTemplateService(IRepository<BaseUrlTemplateRecord> repository)
        {
            _repository = repository;
        }

        public List<BaseUrlTemplateRecord> GetList()
        {
            return _repository.Table.OrderBy(p => p.Position).ToList();
        }

        public BaseUrlTemplateRecord Get(int id)
        {
            return _repository.Get(id);
        }

        public void Create(BaseUrlTemplateRecord record)
        {
            _repository.Create(record);
        }

        public void Update(BaseUrlTemplateRecord record)
        {
            _repository.Update(record);
        }

        public void Delete(BaseUrlTemplateRecord record)
        {
            _repository.Delete(record);
        }

    }
}