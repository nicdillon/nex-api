namespace NexAPI.Models
{
    public class SteamUser
    {
        public required string SteamID { get; set; }
        public int CommunityVisibilityState { get; set; }
        public int ProfileState { get; set; }
        public required string PersonName { get; set; }
        public required string ProfileURL { get; set; }
        public required string Avatar { get; set; }
        public string? AvatarMedium { get; set; }
        public string? AvatarFull { get; set; }
        public string? AvatarHash { get; set; }
        public long LastLogoff { get; set; }
        public int PersonaState { get; set; }
        public string? PrimaryClanID { get; set; }
        public long TimeCreated { get; set; }
        public int PersonaStateFlags { get; set; }
        public string? LocCountryCode { get; set; }
    }
}
