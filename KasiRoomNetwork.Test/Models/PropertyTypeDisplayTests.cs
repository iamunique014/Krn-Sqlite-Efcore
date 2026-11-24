using KasiRoomNetwork.Common.Models.Enums;
using Xunit;

namespace KasiRoomNetwork.Test.Models;

public class PropertyTypeDisplayTests
{
    [Theory]
    [InlineData(PropertyType.RentalUnits, "Rental Units")]
    [InlineData(PropertyType.BackRooms, "Back Rooms")]
    [InlineData(PropertyType.InsideRooms, "Inside Rooms")]
    public void GetDisplayName_EnumValue_ReturnsDisplayLabel(PropertyType propertyType, string expected)
    {
        Assert.Equal(expected, PropertyTypeDisplay.GetDisplayName(propertyType));
    }

    [Theory]
    [InlineData("RentalUnits", "Rental Units")]
    [InlineData("rentalunits", "Rental Units")]
    [InlineData("BackRooms", "Back Rooms")]
    [InlineData("InsideRooms", "Inside Rooms")]
    [InlineData("UnknownType", "Unknown Type")]
    public void GetDisplayName_StoredValue_ReturnsHumanReadableLabel(string storedValue, string expected)
    {
        Assert.Equal(expected, PropertyTypeDisplay.GetDisplayName(storedValue));
    }

    [Fact]
    public void GetDisplayName_NullOrBlank_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, PropertyTypeDisplay.GetDisplayName((string?)null));
        Assert.Equal(string.Empty, PropertyTypeDisplay.GetDisplayName(" "));
        Assert.Equal(string.Empty, PropertyTypeDisplay.GetDisplayName((PropertyType?)null));
    }
}
