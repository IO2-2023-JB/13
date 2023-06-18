using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Mappers
{
    public static class VideoMapper
    {
        public static VideoMetadataDto ToVideoMetadataDto(this VideoModel videoModel) => new()
        {
            Id = videoModel.Id,
            Title = videoModel.Title,
            Description = videoModel.Description,
            Thumbnail = videoModel?.Thumbnail?.Url ?? "", // "" czy null
            AuthorId = videoModel!.CreatorId,
            AuthorNickname = videoModel.Creator.UserName,
            ViewCount = videoModel.ViewCount,
            Tags = videoModel.Tags.Select(t => t.Content).ToList(),
            Visibility = videoModel.IsVisible ? VisibilityEnum.Public : VisibilityEnum.Private,
            ProcessingProgress = videoModel.ProcessingProgress,
            UploadDate = videoModel.UploadDate,
            EditDate = videoModel.EditDate,
            Duration = videoModel.Duration.ToString()
        };
    }
}
