using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Mappers
{
    public static class UserMapper
    {
        public static  UserDto MapUserModelToUserDto(AppUserModel user, UserTypeEnum userType)
        {
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Nickname = user.UserName,
                UserType = userType,
                AvatarImage = Random.Shared.NextDouble() <= 0.2 ? "https://videioblob.blob.core.windows.net/blob1/burger.png" : user?.ProfilePicture?.Url,
                AccountBalance = user?.AccountBalance
            };
            if (userDto.UserType != UserTypeEnum.Creator)
                return userDto;
            if (user.Money is null)
                throw new UserException("Creator doesn't have required properties");
            userDto.SubscriptionsCount = user.Subscribers.Count;
            return userDto;
        }
    }
}
