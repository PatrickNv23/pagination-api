using FuzzySharp;

namespace PaginationResultWebApi.Common;

public static class FuzzySearchHelper
{
    public static async Task<List<SearchResult>> SearchAsync(
        string searchText,
        IEnumerable<string> items,
        int threshold = 70)
    {
        return await Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return [];

            searchText = searchText.Trim().ToLowerInvariant();

            var results = items
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Select(text =>
                {
                    var normalized = text.Trim().ToLowerInvariant();

                    // Fuzzy similarity using WeightedRatio
                    var score = Fuzz.WeightedRatio(searchText, normalized);

                    return new SearchResult
                    {
                        OriginalText = text,
                        SimilarityScore = score
                    };
                })
                .Where(r => r.SimilarityScore >= threshold)
                .OrderByDescending(r => r.SimilarityScore)
                .ToList();

            return results;
        });
    }
}

public class SearchResult
{
    public string OriginalText { get; set; } = string.Empty;
    public int SimilarityScore { get; set; }
}