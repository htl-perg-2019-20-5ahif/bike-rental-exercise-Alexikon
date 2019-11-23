using BikeRental.Model;
using System;

namespace BikeRental
{
    public class CostCalculator : ICostCalculator
    {
        public decimal Calculate(DateTime start, DateTime end, decimal prizeFirstHour, decimal prizeAdditionalHours)
        {
            var duration = end - start;
            if (duration <= TimeSpan.FromMinutes(15))
            {
                return 0;
            }

            var additionalHours = (int) Math.Ceiling((duration.Subtract(TimeSpan.FromHours(1))).TotalHours);
            
            return prizeFirstHour + additionalHours * prizeAdditionalHours;
        }
    }
}
