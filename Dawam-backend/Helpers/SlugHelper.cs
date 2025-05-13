
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Dawam_backend.Helpers
{
   

    public static class SlugHelper
    {
        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove accents and special characters
            var slug = RemoveAccents(input).ToLower();

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Trim hyphens
            slug = slug.Trim('-');

            // Replace multiple hyphens with single
            slug = Regex.Replace(slug, @"-+", "-");

            return slug;
        }

        private static string RemoveAccents(string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                    System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static async Task<string> EnsureUniqueSlugAsync<T>(
            this DbSet<T> dbSet,
            string baseSlug,
            string existingSlug = null) where T : class
        {
            if (string.IsNullOrEmpty(baseSlug))
                baseSlug = "user";

            var slug = baseSlug;
            int counter = 1;

            // Check for existing slugs excluding current user's slug
            var query = dbSet.AsQueryable();
            if (!string.IsNullOrEmpty(existingSlug))
            {
                query = query.Where(e => EF.Property<string>(e, "Slug") != existingSlug);
            }

            while (await query.AnyAsync(e => EF.Property<string>(e, "Slug") == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}
