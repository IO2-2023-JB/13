using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Mappers
{
    public static class UserMapper
    {
        public static  UserDto ToUserDto(this AppUserModel user, UserTypeEnum userType, bool includeBalance = false)
        {
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Nickname = user.UserName,
                UserType = userType,
                AvatarImage = Random.Shared.NextDouble() <= 0.002 ? "https://videioblob.blob.core.windows.net/blob1/burger.png" : user?.ProfilePicture?.Url,
                AccountBalance = includeBalance ? user!.AccountBalance : null
            };
            if (userDto.UserType != UserTypeEnum.Creator)
                return userDto;
            userDto.SubscriptionsCount = user!.SubscribersAmount;
            return userDto;
        }
    }
}
