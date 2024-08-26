namespace Wbskt.Core.Web.Services
{
    public class ChannelDetails : Channel
    {
        public int ChannelId { get; set; }

        public Guid ChannelPublisherId { get; set; }

        public Guid ChannelSubscriberId { get; set; }

        public int ServerId { get; set; }
    }

    public class Channel
    {
        public required string ChannelName { get; set; }

        public int UserId { get; set; }

        public int RetentionTime { get; set; }
    }
}
