using CardGames.Models.Stream;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using RtspClientSharp;
using RtspClientSharp.Rtsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace CardGames.Server.Hubs
{
    public class StreamingHub : Hub
    {
        private RtspClient rtspClient;
        private static List<Channel<StreamFrame>> frameChannles = new List<Channel<StreamFrame>>();
        private static List<CancellationToken> cancellationTokens = new List<CancellationToken>();

        private static DateTime LastFrame = DateTime.UtcNow;

        private static bool Initialized;
        private TimeSpan delay = TimeSpan.FromSeconds(5);
        private CancellationTokenSource tokenSoruce = new CancellationTokenSource();

        private static int FrameCounter = 0;

        private readonly IWebHostEnvironment hostingEnvironment;

        public StreamingHub(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<ChannelReader<StreamFrame>> Counter(
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            if (frameChannles.Count == 0)
            {
                await this.ConnectAsync();
            }
            var channel = Channel.CreateUnbounded<StreamFrame>();

            frameChannles.Add(channel);
            cancellationTokens.Add(cancellationToken);

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            //_ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(
            ChannelWriter<int> writer,
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    await writer.WriteAsync(i, cancellationToken);

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                localException = ex;
            }

            writer.Complete(localException);
        }


        private async Task ConnectAsync()
        {
            //TimeSpan delay = TimeSpan.FromSeconds(5);

            var serverUri = new Uri("rtsp://192.168.0.103:554/videoMain");
            var credentials = new NetworkCredential("admin", "admin");
            var connectionParameters = new ConnectionParameters(serverUri, credentials);
            connectionParameters.RtpTransport = RtpTransportProtocol.TCP;

            this.rtspClient = new RtspClient(connectionParameters);

            this.rtspClient.FrameReceived +=
                async (sender, frame) =>
                {
                    FrameCounter++;
                    var fileName = $"{hostingEnvironment.ContentRootPath}/frames/frame-{FrameCounter}.mp4";
                    await File.WriteAllBytesAsync(fileName, frame.FrameSegment.Array);

                    
                    Func<string,string> outputFileNameBuilder = (number) => { return $"{hostingEnvironment.ContentRootPath}/images/frame-{FrameCounter}.png"; };

                    IMediaInfo info = await FFmpeg.GetMediaInfo(fileName);
                    IVideoStream videoStream = info.VideoStreams.First().SetCodec(VideoCodec.h264);

                    IConversionResult conversionResult = await FFmpeg.Conversions.New()
                        .AddStream(videoStream)
                        .ExtractNthFrame(1, outputFileNameBuilder)
                        .Start();

                    


                    var img = Convert.ToBase64String(frame.FrameSegment.Array);
                    for (int i = 0; i < frameChannles.Count; i++)
                    {
                        var data = new StreamFrame()
                        {
                            Base64ImageString = img,
                            TimeStamp = frame.Timestamp
                        };

                        await frameChannles[i].Writer.WriteAsync(data, cancellationTokens[i]);
                        LastFrame = DateTime.UtcNow;
                    }

                   
                    if(FrameCounter > 50)
                    {
                        File.Delete($"{hostingEnvironment.ContentRootPath}/frames/frame-{FrameCounter-50}.mp4");
                        FrameCounter--;
                    };
                    
                };

            //while (true)
            //{
            Console.WriteLine("Connecting...");
            await rtspClient.ConnectAsync(tokenSoruce.Token);
            Console.WriteLine("Connected.");

            var task = Task.Factory.StartNew(async () =>
            {
                await rtspClient.ReceiveAsync(tokenSoruce.Token);
            }, TaskCreationOptions.LongRunning);


        }
    }
}
