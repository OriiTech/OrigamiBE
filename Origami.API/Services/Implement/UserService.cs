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
            var repo = _unitOfWork.GetRepository<User>();

            var existingUser = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.Email.ToLower() == request.Email.ToLower(),
                asNoTracking: true
            );

            if (existingUser != null)
                throw new BadHttpRequestException("UserExisted");

            var newUser = _mapper.Map<User>(request);

            await repo.InsertAsync(newUser);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newUser.UserId;
        }

        public async Task<GetUserResponse> GetUserById(int id)
        {
            User user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                predicate: x => x.UserId == id,
                include: q => q.Include(u => u.Role),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("UserNotFound");
            return _mapper.Map<GetUserResponse>(user);
        }

        public async Task<bool> UpdateUserInfo(int id, UserInfo request)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var user = await repo.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == id,
            asNoTracking: false
            ) ?? throw new BadHttpRequestException("UserNotFound");


            // Update fields
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                bool emailExists = await repo.AnyAsync(x => x.Email == request.Email);
                if (emailExists)
                    throw new BadHttpRequestException("EmailAlreadyUsed");

                user.Email = request.Email;
            }
            if (!string.IsNullOrEmpty(request.Username))
                user.Username = request.Username;

            user.UpdatedAt = DateTime.UtcNow;
            // Commit the changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
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
