namespace DRD.Models.View
{
    public class ActivityItem
    {
        public int ExitCode { get; set; }
        public string ExitStatus { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long PreviousUserId { get; set; }
        public string PreviousUserName { get; set; }
        public string PreviousEmail { get; set; }
        public string RotationName { get; set; }
        public string ActivityName { get; set; }
        public long RotationId { get; set; }
        public long RotationNodeId { get; set; }
        public string LastActivityStatus { get; set; }
        public ActivityItem() { }
    }
}
