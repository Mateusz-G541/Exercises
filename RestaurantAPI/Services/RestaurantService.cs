using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;
using NLog.Web;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        IEnumerable<RestaurantDto> GetAll();
        int Create(CreateRestaurantDto dto);
        void Delete(int id);
        void Update(int id, UpdateRestaurantDto dto);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RestaurantService(RestaurantDBContext dbContext, IMapper mapper, ILogger<RestaurantService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }
        public void Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = _dbContext
                        .Restaurants
                        .FirstOrDefault(r => r.Id == id);
            if (restaurant == null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
                       
        }
        public void Delete(int id)
        {
            _logger.LogWarning($"Restaurant with id: {id} DELETE action invoked");
            _logger.LogError($"Nlog config error catching test");
            var restaurant = _dbContext
                        .Restaurants
                        .FirstOrDefault(r => r.Id == id);
            if (restaurant == null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
          
        }
        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
               .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
               .FirstOrDefault(r => r.Id == id);
            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");
            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }

        public IEnumerable<RestaurantDto> GetAll()
        {
            var restaurants = _dbContext
                  .Restaurants
                  .Include(r => r.Address)
                  .Include(r => r.Dishes)
                  .ToList();
            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
            return restaurantsDtos;

        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}
