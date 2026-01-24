using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Guide;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Origami.API.Services.Implement
{
    public class GuideService : BaseService<GuideService>, IGuideService
    {
        private readonly IConfiguration _configuration;
        private readonly IBadgeEvaluator _badgeEvaluator;
        private readonly IUploadService _uploadService;

        public GuideService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<GuideService> logger, IMapper mapper,
           IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IUploadService uploadService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
            _uploadService = uploadService;
        }

        private string? ExtractObjectNameFromUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            // Pattern: https://firebasestorage.googleapis.com/v0/b/{bucket}/o/{objectName}?alt=media
            var match = System.Text.RegularExpressions.Regex.Match(url, @"/o/([^?]+)");
            if (match.Success)
            {
                return Uri.UnescapeDataString(match.Groups[1].Value);
            }
            return null;
        }

        private async Task<string?> GetSignedPhotoUrlAsync(string? photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl)) return null;

            try
            {
                var objectName = ExtractObjectNameFromUrl(photoUrl);
                if (objectName != null)
                {
                    // Tạo signed URL với thời hạn 7 ngày
                    return await _uploadService.GetSignedUrlAsync(objectName, TimeSpan.FromDays(7));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create signed URL for photo: {PhotoUrl}", photoUrl);
            }

            // Fallback: trả về URL gốc nếu không tạo được signed URL
            return photoUrl;
        }

        private async Task<List<string>> GetSignedPromoPhotoUrlsAsync(List<string> promoPhotoUrls)
        {
            if (promoPhotoUrls == null || !promoPhotoUrls.Any())
                return new List<string>();

            var signedUrls = new List<string>();
            foreach (var url in promoPhotoUrls)
            {
                var signedUrl = await GetSignedPhotoUrlAsync(url);
                if (signedUrl != null)
                {
                    signedUrls.Add(signedUrl);
                }
            }
            return signedUrls;
        }

        public async Task<int> CreateNewGuide(GuideInfo request)
        {
            var repo = _unitOfWork.GetRepository<Guide>();

            var existingGuide = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.Title.ToLower() == request.Title.ToLower(),
                asNoTracking: true
            );

            if (existingGuide != null)
                throw new BadHttpRequestException("GuideExisted");

            var newGuide = _mapper.Map<Guide>(request);
            newGuide.CreatedAt = DateTime.UtcNow;
            newGuide.UpdatedAt = DateTime.UtcNow;

            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");
            newGuide.AuthorId = userId;

            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var categoryRepo = _unitOfWork.GetRepository<Category>();
                var allCategories = await categoryRepo.GetAllAsync();

                var categories = allCategories
                    .Where(c => request.CategoryIds.Contains(c.CategoryId))
                    .ToList();

                newGuide.Categories = categories;
            }

            await repo.InsertAsync(newGuide);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            await _badgeEvaluator.EvaluateBadgesForUser(userId);

            return newGuide.GuideId;
        }

        public async Task<int> CreateGuideAsync(GuideSaveRequest request)
        {
            int authorId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

            var guideRepo = _unitOfWork.GetRepository<Guide>();
            var stepRepo = _unitOfWork.GetRepository<Step>();
            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var promoRepo = _unitOfWork.GetRepository<GuidePromoPhoto>();
            var previewRepo = _unitOfWork.GetRepository<GuidePreview>();
            var requirementRepo = _unitOfWork.GetRepository<GuideRequirement>();

            //Check title
            if (await guideRepo.AnyAsync(x => x.Title == request.Title))
                throw new BadHttpRequestException("GuideTitleAlreadyExists");

            //Create Guide
            var guide = new Guide
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                Description = request.Description,
                AuthorId = authorId,
                Price = request.Price.Amount,
                PaidOnly = request.Price.PaidOnly,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsNew = true
            };

            await guideRepo.InsertAsync(guide);
            await _unitOfWork.CommitAsync();
            
            //Categories
            if (request.Category?.Any() == true)
            {
                var categoryNames = request.Category!;
                var categories = await categoryRepo.GetListAsync(
                    predicate: x => x.CategoryName != null && categoryNames.Contains(x.CategoryName)
                );
                guide.Categories = categories.ToList();
            }

            //Product Preview
            if (request.ProductPreview != null)
            {
                if (request.ProductPreview.VideoAvailable &&
                    !string.IsNullOrEmpty(request.ProductPreview.VideoUrl))
                {
                    await previewRepo.InsertAsync(new GuidePreview
                    {
                        GuideId = guide.GuideId,
                        VideoUrl = request.ProductPreview.VideoUrl
                    });
                }

                foreach (var photo in request.ProductPreview.PromoPhotos)
                {
                    await promoRepo.InsertAsync(new GuidePromoPhoto
                    {
                        GuideId = guide.GuideId,
                        Url = photo.Url,
                        DisplayOrder = photo.Order
                    });
                }
            }
            //Steps
            foreach (var stepDto in request.Steps.OrderBy(x => x.Order))
            {
                string? imageUrl = null;
                string? videoUrl = null;

                var firstMedia = stepDto.Medias?.OrderBy(m => m.Order).FirstOrDefault();
                if (firstMedia != null)
                {
                    if (firstMedia.Type == 1) imageUrl = firstMedia.Url;
                    if (firstMedia.Type == 2) videoUrl = firstMedia.Url;
                }

                var step = new Step
                {
                    GuideId = guide.GuideId,
                    StepNumber = stepDto.Order,
                    Title = stepDto.Title,
                    Description = stepDto.Description,
                    ImageUrl = imageUrl,
                    VideoUrl = videoUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await stepRepo.InsertAsync(step);
            }

            //Requirements
            if (request.Requirements != null)
            {
                await requirementRepo.InsertAsync(new GuideRequirement
                {
                    GuideId = guide.GuideId,
                    PaperType = request.Requirements.PaperType,
                    PaperSize = request.Requirements.PaperSize,
                    Colors = string.Join("||", request.Requirements.Color),
                    Tools = string.Join("||", request.Requirements.Tools)
                });
            }

            await _unitOfWork.CommitAsync();

            return guide.GuideId;
        }

        public async Task<GetGuideResponse> GetGuideById(int id)
        {
            Guide guide = await _unitOfWork.GetRepository<Guide>().GetFirstOrDefaultAsync(
                predicate: x => x.GuideId == id,
                include: q => q
                    .Include(x => x.Author)
                    .Include(x => x.Origami),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("GuideNotFound");
            return _mapper.Map<GetGuideResponse>(guide);
        }
        public async Task<GetGuideDetailResponse> GetGuideDetailById(int id)
        {
            var guide = await _unitOfWork.GetRepository<Guide>()
                .GetFirstOrDefaultAsync(
                    predicate: x => x.GuideId == id,
                    include: q => q
                        .Include(x => x.Author).ThenInclude(a => a.UserProfile)
                        .Include(x => x.Categories)
                        .Include(x => x.GuideViews)
                        .Include(x => x.GuideRatings)
                        .Include(x => x.Favorites)
                        .Include(x => x.Steps).ThenInclude(s => s.StepTips)
                        .Include(x => x.GuideRequirement),
                    asNoTracking: true
                ) ?? throw new BadHttpRequestException("GuideNotFound");
            var level = guide.Categories
                .FirstOrDefault(c => c.Type == "LEVEL")
                ?.CategoryName;
            var categories = guide.Categories
                .Where(c => c.Type != "LEVEL")
                .Select(c => c.CategoryName ?? string.Empty)
                .ToList();

            var response = new GetGuideDetailResponse
            {
                Id = guide.GuideId,
                Title = guide.Title,
                Subtitle = guide.Subtitle,
                Description = guide.Description,

                Creator = new CreatorDto
                {
                    Id = guide.Author.UserId,
                    Name = guide.Author.Username,
                    Image = guide.Author.UserProfile?.AvatarUrl,
                    Bio = guide.Author.UserProfile?.Bio
                },

                Level = level,
                Category = categories,

                TotalViews = guide.GuideViews.Count,
                TotalReviews = guide.GuideRatings.Count,
                TotalBookmarks = guide.Favorites.Count,

                Rating = BuildRating(guide.GuideRatings),

                Price = new PriceDto
                {
                    Amount = guide.Price ?? 0,
                    PaidOnly = guide.PaidOnly
                },

                ProductPreview = BuildPreview(guide),

                Content = BuildContent(guide),

                CreatedAt = guide.CreatedAt,
                UpdatedAt = guide.UpdatedAt,

                Bestseller = guide.Bestseller,
                Trending = guide.Trending,
                New = guide.IsNew
            };
            return response;

        }
        public async Task IncreaseView(int guideId)
        {
            var viewRepo = _unitOfWork.GetRepository<GuideView>();

            int? userId = GetCurrentUserId();

            string ip = _httpContextAccessor.HttpContext!
                .Connection.RemoteIpAddress!
                .ToString();

            bool existed = await viewRepo.AnyAsync(x =>
                x.GuideId == guideId &&
                (
                    (userId.HasValue && x.UserId == userId)
                ) &&
                x.ViewedAt >= DateTime.UtcNow.AddMinutes(-10)
            );

            if (existed) return;

            await viewRepo.InsertAsync(new GuideView
            {
                GuideId = guideId,
                UserId = userId,
                ViewedAt = DateTime.UtcNow
            });

            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> UpdateGuideInfo(int id, GuideInfo request)
        {
            var repo = _unitOfWork.GetRepository<DataTier.Models.Guide>();
            var guide = await repo.GetFirstOrDefaultAsync(
            predicate: x => x.GuideId == id,
            asNoTracking: false
            ) ?? throw new BadHttpRequestException("GuideNotFound");


            // Update fields
            if (!string.IsNullOrEmpty(request.Title) && request.Title != guide.Title)
            {
                bool titleExists = await repo.AnyAsync(x => x.Title == request.Title && x.GuideId != id);
                if (titleExists)
                    throw new BadHttpRequestException("GuideTitleAlreadyUsed");

                guide.Title = request.Title;
            }
            if (!string.IsNullOrEmpty(request.Description))
                guide.Description = request.Description;

            if (request.Price.HasValue)
                guide.Price = request.Price.Value;

            if (request.OrigamiId.HasValue)
                guide.OrigamiId = request.OrigamiId;

            guide.UpdatedAt = DateTime.UtcNow;
            // Commit the changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IPaginate<GetGuideResponse>> ViewAllGuide(GuideFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Guide>();

            Expression<Func<Guide, bool>> predicate = x =>
                (string.IsNullOrEmpty(filter.Title) || (x.Title != null && x.Title.Contains(filter.Title))) &&
                (string.IsNullOrEmpty(filter.Description) || (x.Description != null && x.Description.Contains(filter.Description))) &&
                (!filter.MinPrice.HasValue || x.Price >= filter.MinPrice.Value) &&
                (!filter.MaxPrice.HasValue || x.Price <= filter.MaxPrice.Value) &&
                (!filter.AuthorId.HasValue || x.AuthorId == filter.AuthorId.Value) &&
                (!filter.OrigamiId.HasValue || x.OrigamiId == filter.OrigamiId.Value) &&
                (!filter.CreatedAt.HasValue || (x.CreatedAt.HasValue && x.CreatedAt.Value.Date == filter.CreatedAt.Value.Date));

            var response = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetGuideResponse>(x),
                predicate: predicate,
                orderBy: q => q.OrderBy(o => o.Title),
                include: q => q
                    .Include(g => g.Steps)
                    .Include(g => g.Categories)
                    .Include(g => g.Author)
                    .Include(g => g.Origami),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
        public async Task<IPaginate<GetGuideCardResponse>> ViewAllGuideCard(GuideCardFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Guide>();
            Expression<Func<Guide, bool>> predicate = x =>
                (string.IsNullOrEmpty(filter.Title) || (x.Title != null && x.Title.Contains(filter.Title))) &&

                (string.IsNullOrEmpty(filter.CreatorName) ||
                    (x.Author.Username != null && x.Author.Username.Contains(filter.CreatorName))) &&

                (!filter.AuthorId.HasValue || x.AuthorId == filter.AuthorId.Value) &&

                (!filter.CategoryId.HasValue ||
                    x.Categories.Any(c => c.CategoryId == filter.CategoryId)) &&

                (!filter.MinPrice.HasValue || x.Price >= filter.MinPrice) &&
                (!filter.MaxPrice.HasValue || x.Price <= filter.MaxPrice) &&

                (!filter.PaidOnly.HasValue || x.PaidOnly == filter.PaidOnly) &&

                (!filter.Bestseller.HasValue || x.Bestseller == filter.Bestseller) &&
                (!filter.Trending.HasValue || x.Trending == filter.Trending) &&
                (!filter.IsNew.HasValue || x.IsNew == filter.IsNew) &&

                (!filter.MinRating.HasValue ||
                    x.GuideRatings.Any() &&
                    x.GuideRatings.Average(r => r.Rating) >= filter.MinRating) &&

                (!filter.MinViews.HasValue ||
                    x.GuideViews.Count >= filter.MinViews);

            var response = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetGuideCardResponse>(x),
                predicate: predicate,
                include: q => q
                    .Include(x => x.Author)
                     .ThenInclude(a => a.UserProfile)
                     .Include(x => x.Categories)
                     .Include(x => x.GuideViews)
                     .Include(x => x.GuideRatings)
                     .Include(x => x.GuidePromoPhotos),
                     orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                     page: pagingModel.page,
                     size: pagingModel.size
             );

            // Convert PromoPhoto URLs to signed URLs
            foreach (var item in response.Items)
            {
                if (item.PromoPhotos != null && item.PromoPhotos.Any())
                {
                    item.PromoPhotos = await GetSignedPromoPhotoUrlsAsync(item.PromoPhotos);
                }
            }

            return response;
        }

        public async Task<IPaginate<GetGuideCardResponse>> ViewMyGuideCards(PagingModel pagingModel)
        {
            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

            var repo = _unitOfWork.GetRepository<Guide>();
            Expression<Func<Guide, bool>> predicate = x => x.AuthorId == userId;

            var response = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetGuideCardResponse>(x),
                predicate: predicate,
                include: q => q
                    .Include(x => x.Author)
                     .ThenInclude(a => a.UserProfile)
                     .Include(x => x.Categories)
                     .Include(x => x.GuideViews)
                     .Include(x => x.GuideRatings)
                     .Include(x => x.GuidePromoPhotos),
                     orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                     page: pagingModel.page,
                     size: pagingModel.size
             );

            // Convert PromoPhoto URLs to signed URLs
            foreach (var item in response.Items)
            {
                if (item.PromoPhotos != null && item.PromoPhotos.Any())
                {
                    item.PromoPhotos = await GetSignedPromoPhotoUrlsAsync(item.PromoPhotos);
                }
            }

            return response;
        }

        public async Task<IPaginate<GetGuideCardResponse>> ViewMyFavoriteGuideCards(PagingModel pagingModel)
        {
            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

            var favoriteRepo = _unitOfWork.GetRepository<Favorite>();
            var guideRepo = _unitOfWork.GetRepository<Guide>();

            // Get favorite guide IDs
            var favorites = await favoriteRepo.GetListAsync(
                predicate: x => x.UserId == userId,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                asNoTracking: true
            );

            var guideIds = favorites.Select(f => f.GuideId).ToList();
            if (!guideIds.Any())
            {
                return new Paginate<GetGuideCardResponse>
                {
                    Items = new List<GetGuideCardResponse>(),
                    Page = pagingModel.page,
                    Size = pagingModel.size,
                    Total = 0,
                    TotalPages = 0
                };
            }

            // Get guides by IDs with pagination
            var skip = (pagingModel.page - 1) * pagingModel.size;
            var take = pagingModel.size;
            var paginatedGuideIds = guideIds.Skip(skip).Take(take).ToList();

            Expression<Func<Guide, bool>> predicate = x => paginatedGuideIds.Contains(x.GuideId);

            var guides = await guideRepo.GetListAsync(
                selector: x => _mapper.Map<GetGuideCardResponse>(x),
                predicate: predicate,
                include: q => q
                    .Include(x => x.Author)
                     .ThenInclude(a => a.UserProfile)
                     .Include(x => x.Categories)
                     .Include(x => x.GuideViews)
                     .Include(x => x.GuideRatings)
                     .Include(x => x.GuidePromoPhotos),
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                asNoTracking: true
            );

            // Maintain order from favorites
            var orderedGuides = paginatedGuideIds
                .Select(id => guides.FirstOrDefault(g => g.Id == id))
                .Where(g => g != null)
                .ToList();

            var totalPages = (int)Math.Ceiling((double)guideIds.Count / pagingModel.size);

            // Convert PromoPhoto URLs to signed URLs
            foreach (var item in orderedGuides)
            {
                if (item.PromoPhotos != null && item.PromoPhotos.Any())
                {
                    item.PromoPhotos = await GetSignedPromoPhotoUrlsAsync(item.PromoPhotos);
                }
            }

            return new Paginate<GetGuideCardResponse>
            {
                Items = orderedGuides,
                Page = pagingModel.page,
                Size = pagingModel.size,
                Total = guideIds.Count,
                TotalPages = totalPages
            };
        }

        private RatingDto BuildRating(ICollection<GuideRating> ratings)
        {
            if (ratings == null || !ratings.Any())
            {
                return new RatingDto
                {
                    Average = 0,
                    Count = 0,
                    Distribution = new RatingDistributionDto()
                };
            }

            return new RatingDto
            {
                Average = Math.Round(ratings.Average(r => r.Rating), 1),
                Count = ratings.Count,
                Distribution = new RatingDistributionDto
                {
                    FiveStars = ratings.Count(r => r.Rating == 5),
                    FourStars = ratings.Count(r => r.Rating == 4),
                    ThreeStars = ratings.Count(r => r.Rating == 3),
                    TwoStars = ratings.Count(r => r.Rating == 2),
                    OneStar = ratings.Count(r => r.Rating == 1),
                }
            };
        }
        private ProductPreviewDto BuildPreview(Guide guide)
        {
            return new ProductPreviewDto
            {
                VideoAvailable = guide.GuidePreview != null
                                 && !string.IsNullOrEmpty(guide.GuidePreview.VideoUrl),

                VideoUrl = guide.GuidePreview?.VideoUrl,

                PromoPhotos = guide.GuidePromoPhotos
                    .OrderBy(p => p.DisplayOrder)
                    .Select(p => new PromoPhotoDto
                    {
                        Order = p.DisplayOrder,
                        Url = p.Url
                    })
                    .ToList()
            };
        }
        private ContentDto BuildContent(Guide guide)
        {
            return new ContentDto
            {
                Steps = guide.Steps
                    .OrderBy(s => s.StepNumber)
                    .Select(s => new StepDto
                    {
                        Order = s.StepNumber,
                        Title = s.Title ?? string.Empty,
                        Description = s.Description,
                        Tips = (s.StepTips == null || !s.StepTips.Any())
                            ? null
                            : string.Join("||", s.StepTips
                                .Select(t => t?.ToString() ?? string.Empty)
                                .Where(t => !string.IsNullOrEmpty(t))
                            ),
                        MediaUrl = string.IsNullOrEmpty(s.VideoUrl)
                            ? new List<MediaDto>()
                            : s.VideoUrl
                                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select((url, idx) => new MediaDto
                                {
                                    Order = idx,
                                    Url = url,
                                    Note = null
                                })
                                .ToList()
                    })
                    .ToList(),

                TotalSteps = guide.Steps.Count,

                Requirements = BuildRequirement(guide.GuideRequirement)
            };
        }
        private RequirementDto BuildRequirement(GuideRequirement? req)
        {
            if (req == null) return new RequirementDto();

            return new RequirementDto
            {
                PaperType = req.PaperType,
                PaperSize = req.PaperSize,

                Color = string.IsNullOrEmpty(req.Colors)
                    ? new List<string>()
                    : req.Colors.Split('|').ToList(),

                Tools = string.IsNullOrEmpty(req.Tools)
                    ? new List<string>()
                    : req.Tools.Split('|').ToList()
            };
        }

        public async Task<int> AddPromoPhotoAsync(int guideId, AddPromoPhotoRequest request)
        {
            // Check if user is authenticated
            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");
            _logger.LogInformation($"AddPromoPhotoAsync: User {userId} attempting to add promo photo to guide {guideId}");

            var guideRepo = _unitOfWork.GetRepository<Guide>();
            var guide = await guideRepo.GetFirstOrDefaultAsync(
                predicate: x => x.GuideId == guideId,
                asNoTracking: false
            );

            if (guide == null)
            {
                _logger.LogWarning($"AddPromoPhotoAsync: Guide {guideId} not found");
                throw new BadHttpRequestException("Guide not found");
            }

            _logger.LogInformation($"AddPromoPhotoAsync: Guide {guideId} found, AuthorId={guide.AuthorId}, CurrentUserId={userId}");

            // Check if user is the author
            if (guide.AuthorId != userId)
            {
                _logger.LogWarning($"AddPromoPhotoAsync: User {userId} is not the author (AuthorId={guide.AuthorId}) of guide {guideId}");
                throw new BadHttpRequestException("You don't have permission to add promo photo to this guide");
            }

            // Validate file
            if (request.PhotoFile == null || request.PhotoFile.Length == 0)
            {
                throw new BadHttpRequestException("Photo file is required");
            }

            // Validate file type (chỉ cho phép image)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(request.PhotoFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new BadHttpRequestException("Only image files are allowed (jpg, jpeg, png, gif, webp)");
            }

            // Validate file size (max 5MB)
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (request.PhotoFile.Length > maxFileSize)
            {
                throw new BadHttpRequestException("File size must be less than 5MB");
            }

            // Upload file lên Firebase Storage
            var uploadedUrl = await _uploadService.UploadAsync(request.PhotoFile, "guide-promo-photos");
            _logger.LogInformation($"Promo photo uploaded for guide {guideId}: {uploadedUrl}");

            var promoRepo = _unitOfWork.GetRepository<GuidePromoPhoto>();
            var promoPhoto = new GuidePromoPhoto
            {
                GuideId = guideId,
                Url = uploadedUrl,
                DisplayOrder = request.DisplayOrder
            };

            await promoRepo.InsertAsync(promoPhoto);
            await _unitOfWork.CommitAsync();

            return promoPhoto.PhotoId;
        }

        public async Task<bool> UpdatePromoPhotoAsync(int guideId, int photoId, UpdatePromoPhotoRequest request)
        {
            // Check if user is authenticated
            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");
            _logger.LogInformation($"UpdatePromoPhotoAsync: User {userId} attempting to update promo photo {photoId} for guide {guideId}");

            var guideRepo = _unitOfWork.GetRepository<Guide>();
            var guide = await guideRepo.GetFirstOrDefaultAsync(
                predicate: x => x.GuideId == guideId,
                asNoTracking: false
            );

            if (guide == null)
            {
                _logger.LogWarning($"UpdatePromoPhotoAsync: Guide {guideId} not found");
                throw new BadHttpRequestException("Guide not found");
            }

            // Check if user is the author
            if (guide.AuthorId != userId)
            {
                _logger.LogWarning($"UpdatePromoPhotoAsync: User {userId} is not the author (AuthorId={guide.AuthorId}) of guide {guideId}");
                throw new BadHttpRequestException("You don't have permission to update promo photo for this guide");
            }

            var promoRepo = _unitOfWork.GetRepository<GuidePromoPhoto>();
            var promoPhoto = await promoRepo.GetFirstOrDefaultAsync(
                predicate: x => x.PhotoId == photoId && x.GuideId == guideId,
                asNoTracking: false
            );

            if (promoPhoto == null)
            {
                _logger.LogWarning($"UpdatePromoPhotoAsync: Promo photo {photoId} not found for guide {guideId}");
                throw new BadHttpRequestException("Promo photo not found");
            }

            // Update photo file if provided
            if (request.PhotoFile != null && request.PhotoFile.Length > 0)
            {
                // Validate file type (chỉ cho phép image)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(request.PhotoFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new BadHttpRequestException("Only image files are allowed (jpg, jpeg, png, gif, webp)");
                }

                // Validate file size (max 5MB)
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (request.PhotoFile.Length > maxFileSize)
                {
                    throw new BadHttpRequestException("File size must be less than 5MB");
                }

                // Upload new file lên Firebase Storage
                var uploadedUrl = await _uploadService.UploadAsync(request.PhotoFile, "guide-promo-photos");
                promoPhoto.Url = uploadedUrl;
                _logger.LogInformation($"UpdatePromoPhotoAsync: New photo uploaded for promo photo {photoId}: {uploadedUrl}");
            }

            // Update display order if provided
            if (request.DisplayOrder.HasValue)
            {
                promoPhoto.DisplayOrder = request.DisplayOrder.Value;
            }

            await _unitOfWork.CommitAsync();
            _logger.LogInformation($"UpdatePromoPhotoAsync: Successfully updated promo photo {photoId} for guide {guideId}");

            return true;
        }

        public async Task<PaymentGuideResponse> PurchaseGuideAsync(PaymentGuideRequest request)
        {
            int buyerId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");
            _logger.LogInformation($"PurchaseGuideAsync: User {buyerId} attempting to purchase guide {request.GuideId}");

            var guideRepo = _unitOfWork.GetRepository<Guide>();
            var guide = await guideRepo.GetFirstOrDefaultAsync(
                predicate: x => x.GuideId == request.GuideId,
                asNoTracking: false
            );

            if (guide == null)
            {
                throw new BadHttpRequestException("Guide not found");
            }

            // Kiểm tra guide có phải paid_only không
            if (!guide.PaidOnly || !guide.Price.HasValue || guide.Price.Value <= 0)
            {
                throw new BadHttpRequestException("This guide is not available for purchase");
            }

            // Kiểm tra user không phải là author
            if (guide.AuthorId == buyerId)
            {
                throw new BadHttpRequestException("You cannot purchase your own guide");
            }

            var price = guide.Price.Value;

            // Lấy wallet của buyer
            var walletRepo = _unitOfWork.GetRepository<Wallet>();
            var buyerWallet = await walletRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == buyerId,
                asNoTracking: false
            );

            if (buyerWallet == null)
            {
                // Tạo wallet mới nếu chưa có
                buyerWallet = new Wallet
                {
                    UserId = buyerId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await walletRepo.InsertAsync(buyerWallet);
                await _unitOfWork.CommitAsync();
            }

            // Kiểm tra số dư
            if (buyerWallet.Balance == null || buyerWallet.Balance < price)
            {
                throw new BadHttpRequestException("Insufficient balance");
            }

            // Lấy wallet của author
            var authorWallet = await walletRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == guide.AuthorId,
                asNoTracking: false
            );

            if (authorWallet == null)
            {
                // Tạo wallet mới cho author nếu chưa có
                authorWallet = new Wallet
                {
                    UserId = guide.AuthorId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await walletRepo.InsertAsync(authorWallet);
                await _unitOfWork.CommitAsync();
            }

            // Lấy wallet của admin (walletId 4, userId 22)
            var adminWallet = await walletRepo.GetFirstOrDefaultAsync(
                predicate: x => x.WalletId == 4 && x.UserId == 22,
                asNoTracking: false
            );

            if (adminWallet == null)
            {
                throw new BadHttpRequestException("Admin wallet not found");
            }

            // Tính toán số tiền
            var authorAmount = price * 0.9m; // 90% cho author
            var adminAmount = price * 0.1m; // 10% cho admin

            // Trừ tiền từ buyer
            buyerWallet.Balance = (buyerWallet.Balance ?? 0) - price;
            buyerWallet.UpdatedAt = DateTime.UtcNow;

            // Thêm tiền cho author
            authorWallet.Balance = (authorWallet.Balance ?? 0) + authorAmount;
            authorWallet.UpdatedAt = DateTime.UtcNow;

            // Thêm tiền cho admin
            adminWallet.Balance = (adminWallet.Balance ?? 0) + adminAmount;
            adminWallet.UpdatedAt = DateTime.UtcNow;

            // Tạo transactions
            var transactionRepo = _unitOfWork.GetRepository<Transaction>();
            var now = DateTime.UtcNow;

            // Transaction từ buyer đến author (90%)
            var transactionToAuthor = new Transaction
            {
                SenderWalletId = buyerWallet.WalletId,
                ReceiverWalletId = authorWallet.WalletId,
                Amount = authorAmount,
                TransactionType = "Payment",
                Status = "Success",
                CreatedAt = now
            };
            await transactionRepo.InsertAsync(transactionToAuthor);

            // Transaction từ buyer đến admin (10%)
            var transactionToAdmin = new Transaction
            {
                SenderWalletId = buyerWallet.WalletId,
                ReceiverWalletId = adminWallet.WalletId,
                Amount = adminAmount,
                TransactionType = "Payment",
                Status = "Success",
                CreatedAt = now
            };
            await transactionRepo.InsertAsync(transactionToAdmin);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation($"PurchaseGuideAsync: Successfully processed payment. Buyer: {buyerId}, Guide: {request.GuideId}, Amount: {price}, Author: {authorAmount}, Admin: {adminAmount}");

            return new PaymentGuideResponse
            {
                Success = true,
                Message = "Payment successful",
                TransactionId = transactionToAuthor.TransactionId
            };
        }

    }
}
