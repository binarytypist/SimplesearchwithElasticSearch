using Abp.Domain.Repositories;
using Elasticsearch.Net;
using GeorgDuncker.Vessels.Values;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeorgDuncker.Vessels
{
    public class VesselSearchAppService :  IVesselSearchAppService
    {

        private const string ComponentIndex = "vesselindex";
        private readonly IRepository<Vessel, Guid> _repository;

        public VesselSearchAppService(IRepository<Vessel, Guid> repository)
        {
            this._repository = repository;

        }

       private async Task UpdateIndex()
        {
            var index = await ElasticClient.Indices.ExistsAsync(ComponentIndex);
            if (index.Exists)
            {
                await ElasticClient.Indices.DeleteAsync(ComponentIndex);
            }
            else
            {
                var createResult =
                   await ElasticClient.Indices.CreateAsync(ComponentIndex, c => c
                   .Map<VesselMappingDocument>(m => m.AutoMap())
                );
            }
            ElasticClient.Bulk(b => b
            .Index(ComponentIndex)
            .IndexMany(_repository.GetAllIncluding(vessel => vessel.GeneralInformation.Type)
            .Select(record => new VesselMappingDocument(record)).ToList())
            .Refresh(Refresh.WaitFor)
        );
        }

        public async Task<ISearchResponse<VesselMappingDocument>> SearchVesselAsync(string query)
        {
            await UpdateIndex();
            ISearchResponse<VesselMappingDocument> response = null;

            response = ElasticClient.Search<VesselMappingDocument>(r => r
                     .From(0)
                     .Size(2000)
                     .Query(q => q
                     .Bool(b => b
                     .Should(
                       s => s.Wildcard(entity => entity.Name, query.ToLower()),
                       s => s.Wildcard(entity => entity.Manager, query.ToLower()),
                       s => s.Wildcard(entity => entity.OwningCompany, query.ToLower()),
                       s => s.Wildcard(entity => entity.IMO, query.Trim()),
                       s => s.Wildcard(entity => entity.GT, query.Trim()),
                       s => s.Wildcard(entity => entity.DWT, query.Trim()),
                       s => s.Wildcard(entity => entity.Type.Label, query.ToLower())
                   )) ||
                 (q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(Infer.Field<VesselMappingDocument>(ff => ff.Name))
                        .Field(Infer.Field<VesselMappingDocument>(ff => ff.Manager))
                        .Field(Infer.Field<VesselMappingDocument>(ff => ff.OwningCompany))
                        .Field(Infer.Field<VesselMappingDocument>(ff => ff.Type))
                    )
                    .Operator(Operator.Or)
                    .Query(query)
                   ) && +q
                .Term("_index", ComponentIndex)
                 )
               )); 

            return response;
      
        }

        private static ElasticClient ElasticClient
        {
            get
            {
                var node = new Uri("http://localhost:9200");
                var node1 = new Uri("http://localhost:9204");
                var connectionPool = new StaticConnectionPool(new List<Uri>() { node });
                var setting = new ConnectionSettings(connectionPool)
                    .DefaultIndex(ComponentIndex)
                    .DefaultMappingFor<GeneralInformation>(i => i.IndexName(ComponentIndex).RelationName("Componenttype"))
                    .DefaultFieldNameInferrer(f => f)
                    .DisableDirectStreaming()
                    .PrettyJson();
                    return new ElasticClient(setting);
            }
        }
    }
}
