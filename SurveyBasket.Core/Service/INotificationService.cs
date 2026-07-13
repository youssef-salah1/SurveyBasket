namespace SurveyBasket.Core.Service;

public interface INotificationService
{
    Task SendNewPollsNotification(int? pollId = null);
}