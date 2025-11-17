namespace Origami.API.Services.Interfaces
{
    public interface IBadgeEvaluator
    {
        Task EvaluateBadgesForUser(int userId);
    }
}
