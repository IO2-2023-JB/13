using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Data.Repositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using NReco.VideoConverter;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.Enums;
using SixLabors.ImageSharp.ColorSpaces.Conversion;

namespace MyWideIO.API.BackgroundProcessing
{
    public class VideoProcessingBackgroundService : BackgroundService
    {
        public IBackgroundTaskQueue<VideoProcessWorkItem> TaskQueue { get; }
        private readonly IServiceProvider _serviceProvider;
        private readonly IVideoStorageService _videoStorageService;
        private readonly IImageStorageService _imageStorageService;

        public VideoProcessingBackgroundService(IBackgroundTaskQueue<VideoProcessWorkItem> taskQueue, IServiceProvider serviceProvider, IVideoStorageService videoStorageService, IImageStorageService imageStorageService)
        {
            TaskQueue = taskQueue;
            _serviceProvider = serviceProvider;
            _videoStorageService = videoStorageService;
            _imageStorageService = imageStorageService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);
                Console.WriteLine("Dequeued an workItem");

                try
                {
                    await ProcessVideo(workItem, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString() + "\nError occurred executing {WorkItem}.");
                }
            }
        }
        private async Task ProcessVideo(VideoProcessWorkItem workItem, CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            IVideoRepository _videoRepository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
            VideoModel video = await _videoRepository.GetAsync(workItem.VideoId) ?? throw new VideoNotFoundException();
            try
            {

                if (video.ProcessingProgress != ProcessingProgressEnum.Uploaded)
                    throw new VideoException("processing progress wasn't 'uploaded'");
                video.ProcessingProgress = ProcessingProgressEnum.Processing;
                await _videoRepository.UpdateAsync(video);
                using var convertedVideoStream = ConvertVideoStreamToMp4(workItem); // nie trzeba recznie dispose
                var uploadVideoTask = _videoStorageService.UploadVideoFileAsync(workItem.VideoId, convertedVideoStream, stoppingToken); // 
                if (video.Thumbnail is null)
                {
                    using var thumbnailStream = GetThumbnail(workItem); // nie trzeba recznie dispose
                    video.Thumbnail = await _imageStorageService.UploadImageAsync(thumbnailStream, video.Id.ToString());
                }
                await uploadVideoTask; // szybciej
                video.ProcessingProgress = ProcessingProgressEnum.Ready;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error while processing");
                video.ProcessingProgress = ProcessingProgressEnum.FailedToProcess; // failed to process
                throw;
            }
            finally
            {
                File.Delete(workItem.fileName);
                Console.WriteLine("Finished work item");
                await _videoRepository.UpdateAsync(video);
            }
        }
        private Stream GetThumbnail(VideoProcessWorkItem workItem)
        {
            var ffmpeg = new FFMpegConverter();
            var outputThumbnailStream = new MemoryStream();
            ffmpeg.GetVideoThumbnail(workItem.fileName, outputThumbnailStream);
            outputThumbnailStream.Position = 0;
            return outputThumbnailStream;
        }

        private Stream ConvertVideoStreamToMp4(VideoProcessWorkItem workItem)
        {
            var ffmpeg = new FFMpegConverter();
            var outputVideoStream = new MemoryStream();
            ffmpeg.ConvertMedia(workItem.fileName, outputVideoStream, Format.mp4);
            outputVideoStream.Position = 0;
            return outputVideoStream;
        }
    }
}
