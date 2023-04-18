using MyVideIO.Data;
using MyWideIO.API.Data.IRepositories;
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
    }

}
