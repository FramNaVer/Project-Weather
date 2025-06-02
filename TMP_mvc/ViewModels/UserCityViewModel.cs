using TMP_mvc.Models;

namespace TMP_mvc.ViewModels
{
    public class UserCityViewModel
    {
        public List<City> AllCities { get; set; } = new();
        public List<UserCity> FollowedCities { get; set; } = new();
    }
}
