using MongoDB.Driver;
using PathologySuite.Shared.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathologySuite.Shared.Models;
using System.Net.Sockets;

namespace PathologySuite.Shared.Services
{
    public class WsiDbService
    {
        private readonly IMongoCollection<WholeSlideImage> _wsis;

        public WsiDbService(IPathologySuiteDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _wsis = database.GetCollection<WholeSlideImage>(settings.WSIsCollectionName);
        }

        public async Task<List<WholeSlideImage>> GetAsync() => (await _wsis.FindAsync(wsi => true)).ToList();

        public async Task<WholeSlideImage> GetAsync(string id) => (await _wsis.FindAsync<WholeSlideImage>(wsi => wsi.Id == id)).FirstOrDefault();

        public async Task<WholeSlideImage> CreateAsync(WholeSlideImage wsi)
        {
            await _wsis.InsertOneAsync(wsi);
            return wsi;
        }

        public async Task UpdateAsync(string id, WholeSlideImage wsiIn) => await _wsis.ReplaceOneAsync(wsi => wsi.Id == id, wsiIn);

        public async Task CreateOrUpdateAsync(string id, WholeSlideImage wsi)
        {
            await _wsis.ReplaceOneAsync(filter: wsi => wsi.Id == id, replacement: wsi, options: new ReplaceOptions() { IsUpsert = true });
        }

        public async Task RemoveAsync(WholeSlideImage wsiIn) => await _wsis.DeleteOneAsync(wsi => wsi.Id == wsiIn.Id);

        public async Task RemoveAsync(string id) => await _wsis.DeleteOneAsync(wsi => wsi.Id == id);
    }
}
