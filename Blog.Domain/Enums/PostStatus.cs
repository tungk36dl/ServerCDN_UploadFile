using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Enums
{
    public enum PostStatus
    {
        Draft,
        Published
    }

    public static class PostStatusHelper
    {
        // Bản đồ chữ → Enum (bất phân hoa-thường)
        private static readonly Dictionary<string, PostStatus> _map =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["draft"] = PostStatus.Draft,
                ["published"] = PostStatus.Published
            };

        /// <summary>
        /// Parse bắt buộc thành công – ném ArgumentException nếu sai.
        /// </summary>
        public static PostStatus Parse(string? value)
            => _map.TryGetValue(value ?? string.Empty, out var status)
               ? status
               : throw new ArgumentException(
                     $"Giá trị PostStatus không hợp lệ: “{value}”. " +
                     $"Hợp lệ: {string.Join(", ", _map.Keys)}", nameof(value));

        /// <summary>
        /// Parse an toàn – trả về true/false.
        /// </summary>
        public static bool TryParse(string? value, out PostStatus status)
            => _map.TryGetValue(value ?? string.Empty, out status);

        /// <summary>
        /// Chuyển Enum → string chuẩn để lưu xuống DB hoặc trả về API.
        /// </summary>
        public static string ToDatabase(this PostStatus status) => status switch
        {
            PostStatus.Draft => "draft",
            PostStatus.Published => "published",
            _ => status.ToString().ToLowerInvariant()
        };
    }
}
