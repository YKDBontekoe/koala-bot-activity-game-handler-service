namespace Koala.ActivityGameHandlerService.Models.Steam;

public class GameData
{
    public string? Title { get; set; }
    public string? Price { get; set; }
    public string? Description { get; set; }
    public string?[]? Tags { get; set; }
    public string? ReleaseDate { get; set; }
    public string? RecentReviewScore { get; set; }
    public string? AllTimeReviewScore { get; set; }
    public GameAreaDetail[]? GameAreaDetails { get; set; }
    public string?[]? Developers { get; set; }
}
