using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }
        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
        {
            //can delete after adding attribute [ApiController]
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            _restaurantService.Update(id, dto);

            return Ok();
        }
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _restaurantService.Delete(id);

            return Ok();

        }
        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            //can delete after adding attribute [ApiController]
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var id = _restaurantService.Create(dto);

            return Created($"/api/restaurant/{id}", null);
        }
        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantsDtos = _restaurantService.GetAll();
            return Ok(restaurantsDtos);
        }
        [HttpGet("{id}")]
        public ActionResult<Restaurant> Get([FromRoute] int id)
        {
            var restaurant = _restaurantService.GetById(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(restaurant);
            }
        }
    }
}
