using ANG_Assessment.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace ANG_Assessment.DB.DBUtil
{
    public class AlertSubscribeDBUtil(AppDBContext db)
    {
        private readonly AppDBContext _db = db;

        public async Task<bool> SubscribeAsync(string email, string location, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required.", nameof(location));

            var existing = await _db.Set<AlertSubscribe>()
                .FirstOrDefaultAsync(x => x.SubscribeEmail == email && x.Location == location, cancellationToken);

            if (existing != null)
                return false;

            var subscribe = new AlertSubscribe
            {
                SubscribeEmail = email,
                Location = location,
                SubscribeDate = DateTime.UtcNow
            };

            await _db.AddAsync(subscribe, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> UnsubscribeAsync(string email, string location, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required.", nameof(location));

            var existing = await _db.Set<AlertSubscribe>()
                .FirstOrDefaultAsync(x => x.SubscribeEmail == email && x.Location == location, cancellationToken);

            if (existing == null)
                return false;

            _db.Remove(existing);
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
