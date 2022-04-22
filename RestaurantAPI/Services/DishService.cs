using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
        DishDto GetById(int restaurantId, int dishId);
        List<DishDto> GetAll(int restaurantId);

        void RemoveAll(int restaurantId);
        void Remove(int restaurantId, int dishId);
    }
    public class DishService : IDishService
    {
        private readonly RestaurantDBContext _context;
        private readonly IMapper _mapper;

        public DishService  (RestaurantDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public DishDto Dish { get; set; }
        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishEnt = _mapper.Map<Dish>(dto);
            dishEnt.RestaurantId = restaurantId;

            _context.Dishes.Add(dishEnt);
            _context.SaveChanges();

            return dishEnt.Id;


        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishId);

            if(dish == null || dish.RestaurantId != restaurantId) throw new NotFoundException("Dish not found");
            {
                var dishDto = _mapper.Map<DishDto>(dish);

                return dishDto;
            }
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant =  GetRestaurantById(restaurantId);

             var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
            return dishDtos;

        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();
        }


        public void Remove(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishId);

            if (dish == null || dish.RestaurantId != restaurantId) throw new NotFoundException("Dish not found");
            _context.Remove(dish);
            _context.SaveChanges();
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context.Restaurants.Include(r => r.Dishes).
                               FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant == null) throw new NotFoundException("Restaurant not found");
            return restaurant;
        }
    }
}
