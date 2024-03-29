﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;
using NLog.Web;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RestaurantAPI.Authorization;
using System.Linq.Expressions;
using System;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery restaurantQuery);
        int Create(CreateRestaurantDto dto);
        void Delete(int id);
        void Update(int id, UpdateRestaurantDto dto);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;


        public RestaurantService(RestaurantDBContext dbContext, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
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

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirment(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
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

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirment(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
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

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery restaurantQuery)
        {

            var baseQuery = _dbContext
                  .Restaurants
                  .Include(r => r.Address)
                  .Include(r => r.Dishes)
                  .Where(r => restaurantQuery.SearchPhrase == null || (r.Name.ToLower().Contains(restaurantQuery.SearchPhrase.ToLower()) || r.Description.ToLower().Contains(restaurantQuery.SearchPhrase.ToLower())));

            if (string.IsNullOrEmpty(restaurantQuery.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name), r => r.Name },
                    {nameof(Restaurant.Description), r => r.Description },
                    {nameof(Restaurant.Category), r => r.Category }

                };
                var selectedColumn = columnsSelector[restaurantQuery.SortBy];
                baseQuery = restaurantQuery.SortDirection == SortDirection.Ascending ?
                    baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = baseQuery
                 .Skip(restaurantQuery.PageSize * (restaurantQuery.PageNumber - 1))
                 .Take(restaurantQuery.PageSize)
                  .ToList();

            var totalItemsCount = baseQuery.Count();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantsDtos, totalItemsCount, restaurantQuery.PageSize, restaurantQuery.PageNumber);
            return result;

        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}
