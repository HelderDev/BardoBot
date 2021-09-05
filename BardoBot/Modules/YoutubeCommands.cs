using BardoBot.Services;
using CliWrap;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using SharpLink;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
namespace BardoBot.Modules
{
    public class YoutubeCommands : ModuleBase
    {

         private async Task<Stream> DownloadVideo(string url)
        {
            ///TODO: APAGAR ARQUIVOS
            ///

            var file = Directory.GetFiles(@"D:\Repos\BardoBot\BardoBot\bin\Debug\netcoreapp3.1").Where(f => f.Contains(".mp4"));
            File.Delete(@"D:\Repos\BardoBot\BardoBot\bin\Debug\netcoreapp3.1\video.mp4");
            var youtube = new YoutubeClient();

            // You can specify both video ID or URL
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);

            // ...or highest bitrate audio-only stream
            var streamInfo = streamManifest.GetAudioOnlyStreams()
                 .Where(s => s.Container == Container.Mp4)
                .GetWithHighestBitrate();
            var stream = youtube.Videos.Streams.GetAsync(streamInfo);

            //await youtube.Videos.Streams.DownloadAsync(streamInfo, $"video.{streamInfo.Container}");

            return await youtube.Videos.Streams.GetAsync(streamInfo);

        }

        [Command("join")]
        public async Task Join(IAudioChannel channel = null) //SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {

            //  IAudioClient audioClient = await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();

            var UserCheck = Context.Guild.GetUserAsync(Context.User.Id);
            channel ??= (Context.Message.Author as IGuildUser)?.VoiceChannel;
            var roleCheck = Context.Guild.Roles.FirstOrDefault(x => x.Name == "DJ");

            //if (UserCheck.Result.RoleIds.ToList().Contains(roleCheck.Id))
            //{
                if (channel == null)
                {
                    await Context.Message.DeleteAsync();
                    await ReplyAsync("You need to be in a voice channel, or pass one as an argument.");
                    return;
                }
                else
                {

                IAudioClient audioClient = await channel.ConnectAsync();
                //IAudioClient audioClient = channel.ConnectAsync().Result;



            }
            //}
            //else
            //{
            //    await Context.Message.DeleteAsync();
            //    await ReplyAsync("Insufficient Permissions");
            //}
        }


        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd(string song, IAudioChannel channel = null)
        {
            if (channel == null)
            {
                channel = (Context.User as IVoiceState).VoiceChannel;
            }
            var stream = await DownloadVideo(song);
            var service = new AudioHandler();
            await service.SendAudioAsync(Context.Guild, channel, Context.Channel, stream);
        }

  
    }
}
