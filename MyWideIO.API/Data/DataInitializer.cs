using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;

namespace MyWideIO.API.Data
{
    public static class DataInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider, string base64Image)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<UserRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUserModel>>();
            var imageStorageService = serviceProvider.GetRequiredService<IImageStorageService>();


            string[] roleNames = Enum.GetValues(typeof(UserTypeEnum)).Cast<UserTypeEnum>().Select(t => t.ToString()).ToArray();

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                    await roleManager.CreateAsync(new UserRole(roleName));
            }
            var adminUser = new AppUserModel
            {
                UserName = "Admin",
                Email = "admin@admin.admin",
                Name = "Admin",
                Surname = "Admin"
            };
            if (await userManager.FindByEmailAsync(adminUser.Email) is null)
            {
                var result = await userManager.CreateAsync(adminUser, "admin");
                if (!result.Succeeded)
                {
                    throw (userManager.ErrorDescriber.DuplicateEmail(adminUser.Email).Code == result.Errors.First().Code) ?
                        new DuplicateEmailException() : new UserException(result.Errors.First()?.Code);
                }

                result = await userManager.AddToRoleAsync(adminUser, UserTypeEnum.Administrator.ToString());
                if (!result.Succeeded)
                {
                    throw new UserException(result.Errors.First()?.Code);
                }
            }

            var videoRepository = serviceProvider.GetRequiredService<IVideoRepository>();
            var videos = await videoRepository.GetUploadingUploadedProcessingVideos();
            foreach (var video in videos)
            {
                video.ProcessingProgress = video.ProcessingProgress == ProcessingProgressEnum.Uploading ? ProcessingProgressEnum.FailedToUpload : ProcessingProgressEnum.FailedToProcess;
            }
            await videoRepository.UpdateAsync(videos);

        }
    }
}
