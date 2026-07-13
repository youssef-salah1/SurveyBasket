namespace SurveyBasket.Core.Entities;

public sealed class Vote
{
    public int Id { get; set; }
    public int PollId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime SubmittedIn { get; set; } = DateTime.UtcNow;
    public ICollection<VoteAnswer> Answers { get; set; } = [];

    public Poll Poll { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
}