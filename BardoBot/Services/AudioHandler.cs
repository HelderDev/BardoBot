using Discord;
using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BardoBot.Services
{
    public class AudioHandler
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                // If you add a method to log happenings from this service,
                // you can uncomment these commented lines to make use of that.
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }

        public async Task SendAudioAsync(IGuild guild, IAudioChannel channel, IMessageChannel txtChannel, Stream streamFlush)
        {
            string path = @"https://www.youtube.com/watch?v=Zh5bW-TuA7U";
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            //if (!File.Exists(path))
            //{
            //    await txtChannel.SendMessageAsync("File does not exist.");
            //    return;
            //}
             var client = await channel.ConnectAsync();

            //IAudioClient client = ;
            //if (ConnectedChannels.TryGetValue(guild.Id, out client))
            //{
            //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");


            using (var ffmpeg = CreateProcess(path))
            using (var stream = client.CreatePCMStream(AudioApplication.Music))
            {
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }
            //}
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = @"C:\Users\Helder\Downloads\ffmpeg-2021-09-05-git-a947098558-full_build\bin\ffmpeg.exe",
                Arguments = $"-i {path} -c copy output.mp3",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}