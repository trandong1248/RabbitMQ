using _1.RabbitMq.Producer.Api.Infrastructure.Application.Interfaces;
using System.Threading.Tasks;

namespace _1.RabbitMq.Producer.Api.Infrastructure.Application.Implementation
{
    public class UserNotificationService : IUserNotificationService
    {

        //public async Task<ApiResult<string>> SetReadedAsync(Guid notifyId, string userId)
        //{
        //    var notify = await _userNotificationRepository.FindByIdAsync(notifyId);

        //    if (notify == null) throw new DomainException("Thông báo không tồn tại.");

        //    if (notify.UserId != userId) throw new DomainException("Thông báo không tồn tại.");

        //    if (notify.Status == UserNotificationStatus.UnRead)
        //    {
        //        notify.Status = UserNotificationStatus.Readed;
        //        await _unitOfWork.SaveAsync();
        //    }

        //    return new ApiSuccessResult<string>("Cập nhật trạng thái thành công");
        //}

        //public async Task<ApiResult<string>> SetReadedsAsync(List<Guid> notifyIds, string userId)
        //{
        //    var notifies = await _userNotificationRepository.FindAll(p => notifyIds.Contains(p.ID) && p.UserId == userId).ToListAsync();
        //    if (notifies?.Any() == true)
        //    {
        //        foreach (var notify in notifies)
        //        {
        //            if (notify.Status == UserNotificationStatus.UnRead)
        //            {
        //                notify.Status = UserNotificationStatus.Readed;
        //            }
        //        }
        //        await _unitOfWork.SaveAsync();
        //    }
        //    return new ApiSuccessResult<string>("Cập nhật trạng thái thành công");
        //}
    }
}