using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.User;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{ 
 public class UserService : BaseService<UserService>, IUserService
{
    private readonly IConfiguration _configuration;
    public UserService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<UserService> logger, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<int> CreateNewUser(UserInfo request)
    {
        User newUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Email.Equals(request.Email)
            );

        if (newUser != null) throw new BadHttpRequestException("UserExisted");

        newUser = _mapper.Map<User>(request);

        await _unitOfWork.GetRepository<User>().InsertAsync(newUser);

        bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

        return newUser.UserId;
    }

    public async Task<GetUserResponse> GetUserById(int id)
    {
        Func<IQueryable<User>, IIncludableQueryable<User, object>> include = q => q.Include(u => u.Role);
        User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.UserId.Equals(id), include: include) ??
            throw new BadHttpRequestException("UserNotFound");

        GetUserResponse result = _mapper.Map<GetUserResponse>(user);
        return result;
    }

    public async Task<bool> UpdateUserInfo(int id, UserInfo request)
        {
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(id)) ??
                throw new BadHttpRequestException("UserNotFound");
 
            // Update other fields
            user.Username = string.IsNullOrEmpty(request.Username) ? user.Username : request.Username;
            user.Email = string.IsNullOrEmpty(request.Email) ? user.Email : request.Email;      
            user.UpdatedAt = DateTime.Now;

            // Perform the update in the repository
            _unitOfWork.GetRepository<User>().UpdateAsync(user);

            // Commit the changes
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;

            return isSuccesful;
        }

        public async Task<IPaginate<GetUserResponse>> ViewAllUser(UserFilter filter, PagingModel pagingModel)
        {
            //Func<IQueryable<User>, IIncludableQueryable<User, object>> include = q => q.Include(u => u.Role);

            IPaginate<GetUserResponse> response = await _unitOfWork.GetRepository<User>().GetPagingListAsync(
                selector: x => _mapper.Map<GetUserResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(u => u.Email),
                //include: include,
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}
