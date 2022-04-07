using AutoMapper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(r => r.City, c => c.MapFrom(m => m.Address.City))
                .ForMember(r => r.Street, c => c.MapFrom(m => m.Address.Street))
                  .ForMember(r => r.PostalCode, c => c.MapFrom(m => m.Address.PostalCode));

            CreateMap<Dish, DishDto>();

            CreateMap<CreateRestaurantDto, Restaurant>()
                 .ForMember(r => r.Address,
                 c => c.MapFrom(dto => new Address()
                 {
                     City = dto.City,Street = dto.Street,PostalCode = dto.PostalCode
                 }));

            CreateMap<UpdateRestaurantDto, Restaurant>();
                





        }
    }
}
