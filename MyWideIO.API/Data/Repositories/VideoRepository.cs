using Microsoft.EntityFrameworkCore;
using MyVideIO.Data;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Extensions;
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
        public async Task<bool> ModifyProcessingState(Guid id, ProcessingProgressDto state)
        {
            VideoModel? model = await GetVideoAsync(id);
            if (model == null)
                return false;
            model.ProcessingProgress = state;

            _dbContext.Update(model);
            return true;
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

        public async Task<VideoUploadResponseDto> UploadVideoMetadata(VideoUploadDto videoData, Guid creatorId)
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
                fileName = "test", //TODO do poprawki
                CreatorId = creatorId,
                Thumbnail = "1",
                ProcessingProgress = ProcessingProgressDto.MetadataRecordCreated,
            };
            videoData.Tags.ForEach(t => model.Tags.Add(new TagModel() { Id = new Guid(), Content = t }));

            /*VideoModel model2 = new VideoModel()
            {
                Title = "a",
                Description = "b",
                Duration = 3,
                IsVisible = true,
                PositiveReactions = 0,
                NegativeReactions = 0,
                CreatorId = creatorId,
                fileName = "asdf",
                Thumbnail = "1",
            }; // for testing
            */

            await _dbContext.AddAsync(model);
            await _dbContext.SaveChangesAsync();

            //( Title, Description, Duration, IsVisible, PositiveReactions, NegativeReactions,CreatorId, fileName, Thumbnail)
            return new VideoUploadResponseDto(model.ProcessingProgress, model.Id);
        }

        public async Task UpdateVideoReaction(Guid videoId, Guid viewerId, VideoReactionUpdateDto videoReactionUpdateDto)
        {
            //ViewerLike viewerLike = await _dbContext.Likes
            //.SingleOrDefaultAsync(vl => vl.ViewerId == viewerId && vl.VideoId == videoId);

            var list = _dbContext.Likes
            .Where(dbItem => (videoId == dbItem.VideoId && viewerId == dbItem.ViewerId)).ToList();

            if (list != null)
            {
                list.ForEach(l => _dbContext.Likes.Remove(l));
            }
            ViewerLike viewerLike = new ViewerLike() { VideoId = videoId, ViewerId = viewerId, Reaction = videoReactionUpdateDto.Value };
            _dbContext.Add(viewerLike);
            _dbContext.SaveChanges();

        }

        public async Task<VideoReactionDto> GetVideoReaction(Guid videoId, Guid viewerId)
        {
            ReactionDto currUserReaction;
            // CurrUser
            var list = _dbContext.Likes
            .Where(dbItem => (videoId == dbItem.VideoId && viewerId == dbItem.ViewerId)).ToList();
            if (list != null)
            {
                list.ForEach(l => { if ((int)l.Reaction == 3) _dbContext.Likes.Remove(l); });
            }
            if (list != null) { currUserReaction = list[0].Reaction; }
            else { currUserReaction = ReactionDto.None; }
            _dbContext.SaveChanges();
            //Count 
            int positiveReviews = 0, negativeReviews = 0;
            list = _dbContext.Likes
           .Where(dbItem => (videoId == dbItem.VideoId)).ToList();
            if (list != null)
            {
                list.ForEach(l =>
                {
                    if (((int)l.Reaction) % 2 != 0) { positiveReviews++; } else { negativeReviews++; } // założenie że ~E l.Reaction == 3
                });
            }

            return new VideoReactionDto() { PositiveCount = positiveReviews, NegativeCount = negativeReviews, CurrentUserReaction = currUserReaction };
        }
    }

}
