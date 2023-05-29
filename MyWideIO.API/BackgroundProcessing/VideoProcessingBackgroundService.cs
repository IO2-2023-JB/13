using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Data.Repositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using NReco.VideoConverter;
using WideIO.API.Models;
using MyWideIO.API.Exceptions;
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
                    await processVideo(workItem, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString() + "\nError occurred executing {WorkItem}.");
                }
            }
        }
        private async Task processVideo(VideoProcessWorkItem workItem, CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var _videoRepository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
            VideoModel video = await _videoRepository.GetVideoAsync(workItem.VideoId) ?? throw new VideoNotFoundException();
            Console.WriteLine("Video found");
            try
            {
                if (video.ProcessingProgress != ProcessingProgressEnum.Uploaded)
                    throw new VideoException("processing progress wasn't 'uploaded'");
                video.ProcessingProgress = ProcessingProgressEnum.Processing;
                await _videoRepository.UpdateVideoAsync(video);
                Console.WriteLine("Started processing");
                if (workItem.Extension == ".mp4")
                {
                    Console.WriteLine("Extension is mp4, so uploading");
                    if(video.Thumbnail is null)
                    {
                        var thumbnailStream = await GetThumbnail(workItem.VideoFile);
                        video.Thumbnail = await _imageStorageService.UploadImageAsync(thumbnailStream, video.Id.ToString());
                        workItem.VideoFile.Position = 0;
                    }
                    await _videoStorageService.UploadVideoFileAsync(workItem.VideoId, workItem.VideoFile);
                    video.ProcessingProgress = ProcessingProgressEnum.Ready;
                }
                else
                {
                    Console.WriteLine($"Extension is {workItem.Extension}, so converting first");
                    var (convertedVideoStream, thumbnailStream) = await ConvertVideoStreamToMp4(workItem.VideoFile, "mp4");
                    Console.WriteLine("Converted stream created");
                    await _videoStorageService.UploadVideoFileAsync(workItem.VideoId, convertedVideoStream);
                    convertedVideoStream.Dispose();
                    video.Thumbnail ??= await _imageStorageService.UploadImageAsync(thumbnailStream, video.Id.ToString());
                    workItem.VideoFile.Dispose();
                    Console.WriteLine($"File uploaded https://videioblob.blob.core.windows.net/video/{workItem.VideoId}.mp4");
                    video.ProcessingProgress = ProcessingProgressEnum.Ready;
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error while processing");
                video.ProcessingProgress = ProcessingProgressEnum.FailedToUpload; // failed to process
                video.Description = e.Message;
                throw;
            }
            finally
            {
                Console.WriteLine("Finished work item");
                await _videoRepository.UpdateVideoAsync(video);
            }
        }
        private async Task<Stream> GetThumbnail(Stream video)
        {
            var ffmpeg = new FFMpegConverter();
            var outputThumbnailStream = new MemoryStream();
            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFilePath)) // nie ladnie
            {
                await video.CopyToAsync(fileStream);

            }
            ffmpeg.GetVideoThumbnail(tempFilePath, outputThumbnailStream);
            File.Delete(tempFilePath);
            outputThumbnailStream.Position = 0;
            return outputThumbnailStream;
        }

            private async Task<(Stream video, Stream thumbnail)> ConvertVideoStreamToMp4(Stream inputStream, string outputFormat)
        {
            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFilePath))
            {
                await inputStream.CopyToAsync(fileStream);
            }
            var ffmpeg = new FFMpegConverter();
            var outputVideoStream = new MemoryStream();
            var outputThumbnailStream = new MemoryStream();
            ffmpeg.ConvertMedia(tempFilePath, outputVideoStream, outputFormat);
            ffmpeg.GetVideoThumbnail(tempFilePath, outputThumbnailStream);
            File.Delete(tempFilePath);
            outputVideoStream.Position = 0;
            outputThumbnailStream.Position = 0;
            return (outputVideoStream, outputThumbnailStream);
        }
    }
}
