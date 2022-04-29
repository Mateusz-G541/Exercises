using Microsoft.AspNetCore.Authorization;
using System;

namespace RestaurantAPI.Authorization
{
    public class MinimumAgeRequirment : IAuthorizationRequirement
    {
        public int MinimumAge { get; }
        

        public MinimumAgeRequirment(int minumumAge)
        {
            MinimumAge = minumumAge;
        }
    }
}
