using BusinessObjects;
using CommonObjects.AppConstants;
using CommonObjects.Pagination;
using MongoDB.Bson;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class EmoteService(IEmoteRepository emoteRepository) : IEmoteService
    {
        public async Task<List<Emote>> GetAllEmotesAsync(CancellationToken ct = default)
           => await emoteRepository.GetAllAsync(ct);

        public async Task<Emote?> GetEmoteByIdAsync(ObjectId id, CancellationToken ct = default)
            => await emoteRepository.GetByIdAsync(id, ct);

        public async Task InsertEmoteAsync(Emote emote, CancellationToken ct = default)
        {
            emote.Id = ObjectId.GenerateNewId();
            emote.CreatedAt = DateTime.UtcNow;
            emote.UpdatedAt = DateTime.UtcNow;

            // default values
            emote.Visibility = new List<string> { "Public"};
            emote.Status = new List<string> { EmoteStatusConstants.ACTIVE};

            await emoteRepository.InsertAsync(emote, ct);
        }

        public async Task<bool> ReplaceEmoteAsync(Emote emote, bool upsert = false, CancellationToken ct = default)
        {
            var existing = await emoteRepository.GetByIdAsync(emote.Id, ct);
            if (existing == null) return false;

            
            existing.Name = emote.Name;
            existing.Tags = emote.Tags ?? new List<string>();
            existing.IsOverlaying = emote.IsOverlaying;
            existing.Visibility = emote.Visibility;
            existing.Status = emote.Status ?? new List<string> { EmoteStatusConstants.ACTIVE};
            existing.UpdatedAt = DateTime.UtcNow;

            return await emoteRepository.ReplaceAsync(existing, upsert, ct);
        }

        public Task<bool> DeleteEmoteByIdAsync(ObjectId id, CancellationToken ct = default)
            => emoteRepository.DeleteByIdAsync(id, ct);

        public Task<bool> DeleteEmoteAsync(Emote emote, CancellationToken ct = default)
            => emoteRepository.DeleteAsync(emote, ct);

        public Task<long> CountEmotesAsync(System.Linq.Expressions.Expression<Func<Emote, bool>>? filter = null, CancellationToken ct = default)
            => emoteRepository.CountAsync(filter, ct);

        public Task<List<Emote>> GetEmotesAsync(System.Linq.Expressions.Expression<Func<Emote, bool>>? filter, CancellationToken ct = default)
            => emoteRepository.GetAsync(filter, ct);

        public async Task<PagedResult<Emote>> GetFilteredEmotesAsync(int page, int pageSize, string userId, string search = "", string tags = "", string[]? filters = null)
        {
            // Get all emotes from repository
            var allEmotes = await emoteRepository.GetAllAsync();
            
            // Convert to queryable for filtering
            var query = allEmotes.AsQueryable();

            // Apply userId => get emote of current user
            if (!string.IsNullOrEmpty(userId))
            {
                // if login => show public emote and public/private emote of login user
                query = query.Where(e => 
                                            e.Visibility.Contains(EmoteVisibilityConstant.PUBLIC) || 
                                            e.OwnerId.ToString().Equals(userId)
                );
            }
            else {
                // if not login => only show public emote
                query = query.Where(e => e.Visibility.Contains(EmoteVisibilityConstant.PUBLIC));
            }
            
            // Apply tags filter
            if (!string.IsNullOrEmpty(tags))
            {
                var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(t => t.Trim().ToLower())
                                 .ToList();

                foreach (var tag in tagList)
                {
                    query = query.Where(e => e.Tags != null && e.Tags.Any(t => t.ToLower().Contains(tag)));
                }
            }

            // Apply additional filters
            if (filters != null && filters.Length > 0)
            {
                foreach (var filter in filters)
                {
                    switch (filter.ToLower())
                    {
                        case "animated":
                            query = query.Where(e => e.Files != null && e.Files.Any(f => f.Format != null && f.Format.ToLower() == "gif"));
                            break;
                        case "static":
                            query = query.Where(e => e.Files == null || e.Files.All(f => f.Format == null || f.Format.ToLower() != "gif"));
                            break;
                        case "overlaying":
                            query = query.Where(e => e.IsOverlaying);
                            break;
                        //case "mine":
                        //    query = query.Where(e => e.OwnerId.Equals(ObjectId.Parse(userId)));
                        //    break;
                            // "exact" is handled in search section above
                    }
                }
            }

            // Order by newest first
            query = query.OrderByDescending(e => e.CreatedAt);

            // Get total count before pagination
            var totalCount = query.Count();

            // Apply pagination
            var emotes = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Emote>
            {
                Items = emotes,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
            };
        }
    }
}