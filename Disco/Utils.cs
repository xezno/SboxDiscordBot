using System;
using System.Diagnostics;
using Discord;
using Discord.WebSocket;

namespace Disco
{
    public class Utils
    {
        /// <summary>
        ///     Log a <see cref="string" /> for debug purposes.
        /// </summary>
        /// <param name="logStr">The <see cref="string" /> to log.</param>
        public static void Log(string logStr)
        {
            var stackFrame = new StackTrace().GetFrame(1);
            var callingMethod = stackFrame.GetMethod();
            Console.WriteLine($"[{callingMethod.Name}] {logStr}");
        }

        /// <summary>
        ///     Builds the default <see cref="Embed" /> used universally throughout the bot.
        /// </summary>
        /// <returns>A Discord <see cref="EmbedBuilder" /> with preset material.</returns>
        public static EmbedBuilder BuildDefaultEmbed()
        {
            var eb = new EmbedBuilder();
            eb.WithColor(new Color(7, 109, 246));
            eb.WithAuthor(ConfigBucket.botName, ConfigBucket.botImage, ConfigBucket.botURL);
            eb.WithTimestamp(DateTime.UtcNow);
            eb.WithFooter(ConfigBucket.prefix + "help for more info");
            return eb;
        }

        public static EmbedBuilder BuildDefaultEmbedWithTime(DateTime startTime, DateTime endTime)
        {
            var eb = BuildDefaultEmbed();
            eb.WithFooter(
                $"{ConfigBucket.prefix}help for more info � Process took {(int) (endTime - startTime).TotalSeconds}s");
            return eb;
        }

        /// <summary>
        ///     Sends an error to an <see cref="ISocketMessageChannel" /> on Discord.
        /// </summary>
        /// <param name="channel">The <see cref="ISocketMessageChannel" /> to send the error to.</param>
        /// <param name="msg">The message <see cref="string" /> related to the error.</param>
        public static async void SendError(ISocketMessageChannel channel, string msg)
        {
            var eb = BuildDefaultEmbed();
            eb.WithColor(new Color(14820902));
            eb.AddField("Error", msg);
            await channel.SendMessageAsync("", false, eb.Build());
        }

        /// <summary>
        ///     Compares two <see cref="string" />s using the Levenshtein Distance algorithm in order to calculate their
        ///     difference.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The difference between the two <see cref="string" />s</returns>
        public static int LevenshteinDistance(string a, string b)
        {
            int aLen = a.Length, bLen = b.Length;
            if (aLen == 0 || bLen == 0) return -1;
            var matrix = new int[aLen + 1, bLen + 1];
            for (var i = 0; i <= aLen; ++i) matrix[i, 0] = i;
            for (var i = 0; i <= bLen; ++i) matrix[0, i] = i;
            for (var j = 1; j <= bLen; ++j)
            for (var i = 1; i <= aLen; ++i)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                var val0 = matrix[i - 1, j] + 1;
                var val1 = matrix[i, j - 1] + 1;
                var val2 = matrix[i - 1, j - 1] + cost;
                matrix[i, j] = Math.Min(val0, Math.Min(val1, val2));
            }

            return matrix[aLen, bLen];
        }
    }
}