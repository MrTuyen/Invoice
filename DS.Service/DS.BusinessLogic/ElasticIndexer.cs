using System;
using System.Configuration;
using Nest;
//using Elasticsearch.Net.ConnectionPool;

namespace DS.BusinessLogic
{
    public class ElasticIndexer
    {
        public static System.Object lockQueue = new System.Object();
        public ElasticClient elasticClient = null;

        public ElasticIndexer()
        {
            string ELASTIC_MASTER_NODE = ConfigurationManager.AppSettings["ES_IP"];
            Uri node = new Uri(ELASTIC_MASTER_NODE);
            ConnectionSettings config = new ConnectionSettings(node);
            elasticClient = new ElasticClient(config);
        }
        
        public ElasticClient IndexClient
        {
            get { return elasticClient; }
        }

        public bool DeleteObject(string index, string type, string id)
        {
            lock (lockQueue)
            {
                
                IDeleteResponse response = elasticClient.Delete(new DeleteRequest(index, type, id));
                if (response != null)
                {
                    if (response.IsValid == true)
                    {
                        return response.IsValid;
                    }
                    else
                    {
                        throw new Exception("ElasticIndexer::DeleteObject::" + response.ServerError.Error);
                    }
                }
                return false;
            }
        }
        //public T Get<T>(string id, string index, string type) where T : class
        //{
        //    var rslGet = elasticClient.Get<T>(id, index, type);
        //    if (rslGet != null)
        //    {
        //        return rslGet.Source;
        //    }
        //    return default(T);
        //}


        //public bool UpdateObject<T>(string index, string type, string id, object data) where T : class
        //{
        //    lock (lockQueue)
        //    {
                 
        //        IUpdateResponse response = elasticClient.Update<T, object>(u => u.Type(type)
        //            .Index(index)
        //            .Id(id)
        //            .Doc(data)
        //            .DocAsUpsert()
        //             );
        //        if (response != null)
        //        {
        //            if (response.IsValid == true)
        //            {
        //                return response.IsValid;
        //            }
        //            else
        //            {
        //                throw new Exception("ElasticIndexer::UpdateObject::" + response.ServerError.Error);
        //            }
        //        }
        //        return false;
        //    }
        //}

        //public bool IndexObject<T>(string index, string type, T data, string id) where T : class
        //{
        //    lock (lockQueue)
        //    { 
        //        IIndexRequest<T> req = new IndexRequest<T>(data);
        //        req.Index = index;
        //        req.Id = id;
        //        req.Type = type;
        //        IIndexResponse response = elasticClient.Index(req);
        //        if (response != null)
        //        {
        //            if (response.IsValid == true)
        //            {
        //                return response.IsValid;
        //            }
        //            else
        //            {
        //                throw new Exception("ElasticIndexer::AddUpdateObject::" + response.ServerError.Error);
        //            }
        //        }
        //        return false;
        //    }
        //}
        #region Singleton

        static ElasticIndexer instance;
        public static ElasticIndexer Current
        {
            get
            {
                return instance ?? (instance = new ElasticIndexer());
            }
        }

        #endregion

    }
}