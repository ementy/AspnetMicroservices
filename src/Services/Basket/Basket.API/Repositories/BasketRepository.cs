using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basketAsString = await _redisCache.GetStringAsync(userName);

            if (string.IsNullOrEmpty(basketAsString))
            {
                return null;
            }

            var basket = JsonConvert.DeserializeObject<ShoppingCart>(basketAsString);
            return basket;
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            var basketAsString = JsonConvert.SerializeObject(basket);
            await _redisCache.SetStringAsync(basket.UserName, basketAsString);

            return await GetBasket(basket.UserName);
        }

        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RemoveAsync(userName);
        }
    }
}
