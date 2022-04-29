using RestaurantAPI.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDBContext _dbContext;
        public RestaurantSeeder(RestaurantDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if(!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants);
                    _dbContext.SaveChanges();
                }
            }
        }
        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant
            {
                Name = "KFC",
                Category = "Fast Food",
                Description = " ",
                ContacteEmail =" sample",
                HasDelivery = true,
                ContacteNumber = "1111111",
                Dishes = new List<Dish>
{
                    new Dish
                    {
                        Name ="Chicken",
                        Description="Most common bird",
                        Price  = 33.29M
                    },
                     new Dish
                    {
                        Name ="Turkey",
                        Description="Almost most common bird",
                        Price  = 66.29M
                    },

                    },
                    Address = new Address
                    {
                        City ="Bochnia",
                        Street= "Solna Gora 38",
                        PostalCode = "32-700"

                    }
                                                    },

                                     new Restaurant
                                {
                                         Name = "McDonald",
                    Category = "Fast Food",
                    Description = "Burgers",
                    ContacteEmail ="burgers@burgers.com",
                    HasDelivery = false,
                    ContacteNumber = "222",
                    Dishes = new List<Dish>
                    {
                        new Dish
                        {
                            Name ="Burger",
                            Description="muuu",
                            Price  = 3.29M
                        },
                         new Dish
                        {
                            Name ="Cheesburger",
                            Description="Cheese",
                            Price  = 6.29M
                        },

                    },
                            Address = new Address
                            {
                                City ="Bochnia",
                                Street= "Solna Gora 37",
                                PostalCode = "32-700"

                            }

            }
            };
            return restaurants;
        }

        private IEnumerable<Role>GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role ()
                {
                    Name = "User"
                },
                 new Role ()
                {
                    Name = "Manaer"
                },
                  new Role ()
                {
                    Name = "Admin"
                }
            };
            return roles;
        }
    }
}
