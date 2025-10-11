using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Core.Entities;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment> GetByIdAsync(Guid id);
        Task<List<Enrollment>> GetByUserIdAsync(Guid userId);
        Task<Enrollment> GetByUserAndCourseAsync(Guid userId, Guid courseId);
        Task<List<Enrollment>> GetAllAsync(int pageNumber, int pageSize);
        Task<Enrollment> CreateAsync(Enrollment enrollment);
        Task<Enrollment> UpdateAsync(Enrollment enrollment);
        Task DeleteAsync(Guid id);
    }

    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly SubscriptionDbContext _context;

        public EnrollmentRepository(SubscriptionDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment> GetByIdAsync(Guid id)
        {
            return await _context.Enrollments.FindAsync(id);
        }

        public async Task<List<Enrollment>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Enrollments
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();
        }

        public async Task<Enrollment> GetByUserAndCourseAsync(Guid userId, Guid courseId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task<List<Enrollment>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Enrollments
                .OrderByDescending(e => e.EnrollmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            enrollment.UpdatedAt = DateTime.UtcNow;
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task DeleteAsync(Guid id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }
    }

    public interface ICouponRepository
    {
        Task<Coupon> GetByCodeAsync(string code);
        Task<Coupon> GetByIdAsync(Guid id);
        Task<List<Coupon>> GetActiveAsync();
        Task<Coupon> CreateAsync(Coupon coupon);
        Task<Coupon> UpdateAsync(Coupon coupon);
        Task<bool> ValidateCouponAsync(string code, Guid userId, decimal amount);
    }

    public class CouponRepository : ICouponRepository
    {
        private readonly SubscriptionDbContext _context;

        public CouponRepository(SubscriptionDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code && c.IsActive);
        }

        public async Task<Coupon> GetByIdAsync(Guid id)
        {
            return await _context.Coupons.FindAsync(id);
        }

        public async Task<List<Coupon>> GetActiveAsync()
        {
            return await _context.Coupons
                .Where(c => c.IsActive && c.ValidTo >= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<Coupon> CreateAsync(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<Coupon> UpdateAsync(Coupon coupon)
        {
            coupon.UpdatedAt = DateTime.UtcNow;
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<bool> ValidateCouponAsync(string code, Guid userId, decimal amount)
        {
            var coupon = await GetByCodeAsync(code);
            if (coupon == null) return false;

            // Check validity dates
            if (DateTime.UtcNow < coupon.ValidFrom || DateTime.UtcNow > coupon.ValidTo)
                return false;

            // Check usage limit
            if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
                return false;

            // Check minimum purchase
            if (coupon.MinimumPurchase.HasValue && amount < coupon.MinimumPurchase.Value)
                return false;

            // Check if user already used this coupon
            var userUsed = await _context.UserCoupons
                .AnyAsync(uc => uc.CouponId == coupon.Id && uc.UserId == userId);
            
            return !userUsed;
        }
    }

    public interface IWishlistRepository
    {
        Task<List<Wishlist>> GetByUserIdAsync(Guid userId);
        Task<Wishlist> GetByUserAndCourseAsync(Guid userId, Guid courseId);
        Task<Wishlist> AddAsync(Wishlist wishlist);
        Task RemoveAsync(Guid userId, Guid courseId);
    }

    public class WishlistRepository : IWishlistRepository
    {
        private readonly SubscriptionDbContext _context;

        public WishlistRepository(SubscriptionDbContext context)
        {
            _context = context;
        }

        public async Task<List<Wishlist>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();
        }

        public async Task<Wishlist> GetByUserAndCourseAsync(Guid userId, Guid courseId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.CourseId == courseId);
        }

        public async Task<Wishlist> AddAsync(Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task RemoveAsync(Guid userId, Guid courseId)
        {
            var wishlist = await GetByUserAndCourseAsync(userId, courseId);
            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();
            }
        }
    }
}