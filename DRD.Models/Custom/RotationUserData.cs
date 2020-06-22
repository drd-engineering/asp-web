namespace DRD.Models.Custom
{
    public class RotationUserData
    {
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public string ActivityName { get; set; }
        public long? UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
    }
}
