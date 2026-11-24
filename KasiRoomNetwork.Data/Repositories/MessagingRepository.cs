using KasiRoomNetwork.Common.ViewModel.Messaging;
using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Domain.Models;
using KasiRoomNetwork.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.Repositories
{
    public class MessagingRepository : IMessagingRepository
    {
        private readonly ApplicationDbContext _db;

        public MessagingRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> CreateConversation(int listingId, string tenantId, string landlordId)
        {
            var existing = await _db.Conversations
                .FirstOrDefaultAsync(c => c.ListingId == listingId && c.TenantId == tenantId);
            if (existing != null)
                return existing.ConversationId;

            var conversation = new Conversation
            {
                ListingId = listingId,
                TenantId = tenantId,
                LandlordId = landlordId
            };
            _db.Conversations.Add(conversation);
            await _db.SaveChangesAsync();
            return conversation.ConversationId;
        }

        public async Task<ConversationViewModel?> GetConversationHeader(int conversationId, string userId)
        {
            var conversation = await _db.Conversations
                .Include(c => c.Listing)
                .ThenInclude(l => l.Photos)
                .Include(c => c.Tenant)
                .Include(c => c.Landlord)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId &&
                    (c.TenantId == userId || c.LandlordId == userId));
            if (conversation == null)
                return null;

            var lastMessage = conversation.Messages.Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.MessageId).FirstOrDefault();
            var other = conversation.TenantId == userId ? conversation.Landlord : conversation.Tenant;
            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == other.Id);
            return new ConversationViewModel
            {
                ConversationId = conversation.ConversationId,
                ListingId = conversation.ListingId,
                ListingTitle = conversation.Listing.Title,
                OtherUserId = other.Id,
                OtherUserName = profile?.FullName ?? other.UserName ?? string.Empty,
                LastMessage = lastMessage?.MessageText ?? string.Empty,
                LastMessageAt = lastMessage?.CreatedAt ?? conversation.CreatedAt,
                UnreadCount = await _db.Messages.CountAsync(m => m.ConversationId == conversationId &&
                    !m.IsDeleted && !m.IsRead && m.SenderId != userId),
                PrimaryPhoto = conversation.Listing.Photos.Where(p => p.IsActive && p.IsPrimary)
                    .Select(p => p.PhotoPath).FirstOrDefault()
            };
        }

        public async Task SendMessage(SendMessageViewModel model)
        {
            var conversation = await _db.Conversations
                .FirstOrDefaultAsync(c => c.ConversationId == model.ConversationId &&
                    (c.TenantId == model.SenderId || c.LandlordId == model.SenderId));
            if (conversation == null)
                return;

            _db.Messages.Add(new Message
            {
                ConversationId = model.ConversationId,
                SenderId = model.SenderId,
                MessageText = model.MessageText
            });
            conversation.LastMessageAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<MessageViewModel>> GetConversationMessages(int conversationId, string userId)
        {
            if (!await UserOwnsConversation(conversationId, userId))
                return Enumerable.Empty<MessageViewModel>();

            return await _db.Messages.Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderBy(m => m.CreatedAt).ThenBy(m => m.MessageId)
                .Select(m => new MessageViewModel
                {
                    MessageId = m.MessageId,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    MessageText = m.MessageText,
                    IsRead = m.IsRead,
                    IsMyMessage = m.SenderId == userId,
                    CreatedAt = m.CreatedAt
                }).ToListAsync();
        }

        public async Task<IEnumerable<ConversationViewModel>> GetInbox(string userId)
        {
            var ids = await _db.Conversations.AsNoTracking()
                .Where(c => c.TenantId == userId || c.LandlordId == userId)
                .Select(c => c.ConversationId).ToListAsync();
            var result = new List<ConversationViewModel>();
            foreach (var id in ids)
            {
                var header = await GetConversationHeader(id, userId);
                if (header != null)
                    result.Add(header);
            }
            return result.OrderByDescending(c => c.LastMessageAt).ToList();
        }

        public async Task MarkConversationRead(int conversationId, string userId)
        {
            var messages = await _db.Messages.Where(m => m.ConversationId == conversationId &&
                !m.IsDeleted && !m.IsRead && m.SenderId != userId &&
                (m.Conversation.TenantId == userId || m.Conversation.LandlordId == userId)).ToListAsync();
            foreach (var message in messages)
                message.IsRead = true;
            await _db.SaveChangesAsync();
        }

        public Task<int> GetUnreadCount(string userId) => _db.Messages.CountAsync(m => !m.IsDeleted &&
            !m.IsRead && m.SenderId != userId &&
            (m.Conversation.TenantId == userId || m.Conversation.LandlordId == userId));

        public Task<bool> UserOwnsConversation(int conversationId, string userId) =>
            _db.Conversations.AnyAsync(c => c.ConversationId == conversationId &&
                (c.TenantId == userId || c.LandlordId == userId));

        public async Task CreateContactLog(int listingId, string userId, string contactMethod, int? conversationId = null)
        {
            _db.ListingContactLogs.Add(new ListingContactLog
            {
                ListingId = listingId,
                UserId = userId,
                ContactMethod = contactMethod,
                ConversationId = conversationId
            });
            await _db.SaveChangesAsync();
        }

        public Task<bool> HasInAppContactLog(int listingId, string userId) =>
            _db.ListingContactLogs.AnyAsync(log => log.ListingId == listingId &&
                log.UserId == userId && log.ContactMethod == "InApp");
    }
}
