using MyVideIO.Data;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models;
using MyWideIO.API.Models.DB_Models;
using WideIO.API.Models;

namespace MyWideIO.API.Data.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VideoRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VideoModel?> GetVideoAsync(Guid id)
        {
            return await _dbContext.Videos.FindAsync(id);
        }

        public void RemoveVideo(VideoModel video)
        {
            _dbContext.Videos.Remove(video);
        }

        public async Task<bool> PutVideoData(Guid id, VideoUploadDto videoData)
        {
            VideoModel? model = await GetVideoAsync(id);
            if (model == null)
                return false;
            model.Title = videoData.Title;
            model.Description = videoData.Description;
            model.Tags = new List<TagModel>();
            videoData.Tags.ForEach(t => model.Tags.Add(new TagModel() { Id = new Guid(), Content = t }));
            model.IsVisible = videoData.Visibility == VisibilityDto.PublicEnum;
            
            // TODO Thumbnail

            _dbContext.Update(model);

            return true;
        }

        Task<VideoUploadResponseDto> IVideoRepository.UploadVideoMetadata(VideoUploadDto videoData, Guid creatorId)
        {
            VideoModel model = new VideoModel()
            {
                Title = videoData.Title,
                Description = videoData.Description,
                Duration = 0,
                Tags = new List<TagModel>(),
                IsVisible = videoData.Visibility == VisibilityDto.PublicEnum,
                PositiveReactions = 0,
                NegativeReactions = 0,
                fileName = "test",
                CreatorId = creatorId,
                Thumbnail = "1",
            };
            videoData.Tags.ForEach(t => model.Tags.Add(new TagModel() { Id = new Guid(), Content = t }));
            
            //VideoModel model2 = new VideoModel()
            //{
            //    Title = "a",
            //    Description = "b",
            //    Duration = 3,
            //    IsVisible = true,
            //    PositiveReactions = 0,
            //    NegativeReactions = 0,
            //    CreatorId = creatorId,
            //    fileName = "asdf",
            //    Thumbnail = "1",
            //}; // for testing

            _dbContext.Add(model);
            _dbContext.SaveChanges();

            //( Title, Description, Duration, IsVisible, PositiveReactions, NegativeReactions,CreatorId, fileName, Thumbnail)
            return Task.FromResult(new VideoUploadResponseDto(ProcessingProgressDto.ReadyEnum, model.Id));
        }
    }

}
