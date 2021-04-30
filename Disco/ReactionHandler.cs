using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;

namespace Disco
{
    public class Reaction
    {
        public RestUserMessage Message { get; set; }
        public int TimeAdded { get; set; }
        public int Timeout { get; set; }
        public Func<object> Handler { get; set; }
        public IEmote Emote { get; set; }
    }

    public class ReactionBuilder
    {
        private Reaction reaction;

        public ReactionBuilder()
        {
            reaction = new Reaction();
        }

        public ReactionBuilder SetMessage(RestUserMessage message)
        {
            reaction.Message = message;
            return this;
        }

        public ReactionBuilder SetTimeout(int timeout)
        {
            reaction.Timeout = timeout;
            return this;
        }

        public ReactionBuilder SetEmote(IEmote emote)
        {
            reaction.Emote = emote;
            return this;
        }

        public ReactionBuilder Reacted(Func<object> handler)
        {
            reaction.Handler = handler;
            return this;
        }

        public ReactionBuilder Await()
        {
            ReactionHandler.Instance.WaitForReaction(reaction);
            return this;
        }
    }

    public class ReactionHandler : Singleton<ReactionHandler>
    {
        private List<Reaction> reactions = new List<Reaction>();

        public void WaitForReaction(Reaction reaction)
        {
            reactions.Add(reaction);
        }

        public static ReactionBuilder BuildReaction()
        {
            return new ReactionBuilder();
        }

        public void CleanReactions()
        {
            lock (reactions)
            {
                var ticksNow = (DateTime.Now - new DateTime(1970, 0, 0)).TotalSeconds;
                for (int i = 0; i < reactions.Count; ++i)
                {
                    var reaction = reactions[i];

                    if ((reaction.TimeAdded + reaction.Timeout) > ticksNow)
                    {
                        reactions.RemoveAt(i);
                    }
                }
            }
        }

        public void TriggerReaction(IUser author, ulong messageId, IEmote emote, DiscoApplication application)
        {
            var reactionMatch = reactions.Find(new Predicate<Reaction>((reaction) =>
            {
                return reaction.Message.Id == messageId && (reaction.Emote == null || reaction.Emote == emote) && author != application.Client?.CurrentUser;
            }));
            if (reactionMatch != null)
            {
                reactionMatch?.Handler.Invoke();
                reactions.Remove(reactionMatch);
            }
        }
    }
}
