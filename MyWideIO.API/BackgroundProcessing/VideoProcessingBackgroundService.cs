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

        public VideoProcessingBackgroundService(IBackgroundTaskQueue<VideoProcessWorkItem> taskQueue, IServiceProvider serviceProvider, IVideoStorageService videoStorageService)
        {
            TaskQueue = taskQueue;
            _serviceProvider = serviceProvider;
            _videoStorageService = videoStorageService;
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
                    await _videoStorageService.UploadVideoFileAsync(workItem.VideoId, workItem.VideoFile);
                    video.ProcessingProgress = ProcessingProgressEnum.Ready;
                }
                else
                {
                    Console.WriteLine($"Extension is {workItem.Extension}, so converting first");
                    var convertedStream = await ConvertVideoStreamToMp4(workItem.VideoFile, workItem.Extension[1..], "mp4");
                    Console.WriteLine("Converting stream created");
                    await _videoStorageService.UploadVideoFileAsync(workItem.VideoId, convertedStream);
                    convertedStream.Dispose();
                    workItem.VideoFile.Dispose();
                    Console.WriteLine($"File uploaded https://videioblob.blob.core.windows.net/video/{workItem.VideoId}.mp4");
                    video.ProcessingProgress = ProcessingProgressEnum.Ready;
                }


            }
            catch(Exception e)
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

        private async Task<Stream> ConvertVideoStreamToMp4(Stream inputStream, string inputFormat, string outputFormat)
        {
            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFilePath))
            {
                await inputStream.CopyToAsync(fileStream);
            }
            var ffmpeg = new FFMpegConverter();
            var outputStream = new MemoryStream();
            ffmpeg.ConvertMedia(tempFilePath, inputFormat, outputStream, outputFormat, null);
            File.Delete(tempFilePath);
            outputStream.Position = 0;
            return outputStream;
        }
    }
}
