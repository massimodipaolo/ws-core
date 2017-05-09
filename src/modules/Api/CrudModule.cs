using System;
using System.Collections.Generic;
using System.Text;
using Nancy;
using core.Data;

namespace core.Api
{
    public class ApiModule : NancyModule 
    {   

        public ApiModule()
        {            
            Get("/hello", _ => "Hello world!");
            Get("/hello2", _ => { return "Hello2 world!"; });
        }
    }

    
    public class CrudModule<T> : NancyModule where T : IEntity
    {
        private static string _prefix = "/api3/";
        protected IRepository<T> _repository;

        public CrudModule(IRepository<T> repository): base(_prefix)
        {
            _repository = repository;
            var TName = typeof(T).Name;

            Get(TName, _ => _repository.List);

            Get(TName + "{id}", _ => _repository.Find(_.id));
        }        
    }
    
}
