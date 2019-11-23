using BikeRental;
using System;
using Xunit;

namespace BikeRentalExercise.Tests
{
    public class CalculatorTest
    {
        [Fact]
        public void CalculateCost()
        {
            // prepare
            var c = new CostCalculator();

            // execute
            var calc0 = c.Calculate(DateTime.Now, DateTime.Now.AddMinutes(1), 1, 2);
            var calc3 = c.Calculate(DateTime.Now, DateTime.Now.AddHours(1).AddMinutes(1), 1, 2);

            // assertions
            Assert.Equal(0, calc0);
            Assert.Equal(3, calc3);
        }
    }
}
