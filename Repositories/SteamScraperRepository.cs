using System.Text;
using HtmlAgilityPack;
using Koala.ActivityGameHandlerService.Models;
using Koala.ActivityGameHandlerService.Models.Steam;
using Koala.ActivityGameHandlerService.Repositories.Interfaces;

namespace Koala.ActivityGameHandlerService.Repositories;

public class SteamScraperRepository : IScraperRepository
{
    public async Task<GameData?> GetGameDataFromUrl(string url)
    {
        var document = await LoadDocumentFromWebAsync(url);
        if (document.DocumentNode == null)
        {
            return null;
        }
        
        var gameData = new GameData
        {
            Title = ExtractTitle(document),
            Price = ExtractPrice(document),
            Description = ExtractDescription(document),
            Tags = ExtractTags(document),
            Developers = ExtractDevelopers(document),
            ReleaseDate = ExtractReleaseDate(document),
            RecentReviewScore = ExtractRecentReviewScore(document),
            AllTimeReviewScore = ExtractAllTimeReviewScore(document),
            GameAreaDetails = ExtractGameAreaDetails(document)
        };

        return gameData;
    }

    private static async Task<HtmlDocument> LoadDocumentFromWebAsync(string url)
    {
        var web = new HtmlWeb();
        return await web.LoadFromWebAsync(url);
    }

    private static string? ExtractTitle(HtmlDocument document)
    {
        var titleNode = document.DocumentNode.SelectSingleNode("//div[@id='appHubAppName']");
        return titleNode != null ? RemoveNewLinesAndTabs(titleNode.InnerText) : null;
    }

    private static string? ExtractPrice(HtmlDocument document)
    {
        var priceNode = document.DocumentNode.SelectSingleNode("//div[@class='game_purchase_price price']") ??
                        document.DocumentNode.SelectSingleNode("//div[@class='discount_original_price']");
        return priceNode != null ? RemoveNewLinesAndTabs(priceNode.InnerText) : null;
    }

    private static string? ExtractDescription(HtmlDocument document)
    {
        var descriptionNode = document.DocumentNode.SelectSingleNode("//div[@class='game_description_snippet']");
        return descriptionNode != null ? RemoveNewLinesAndTabs(descriptionNode.InnerText) : null;
    }

    private static string?[]? ExtractTags(HtmlDocument document)
    {
        var tagNodes = document.DocumentNode.SelectNodes("//a[@class='app_tag']");
        if (tagNodes == null) return null;

        var tags = new string?[tagNodes.Count];
        for (var i = 0; i < tagNodes.Count; i++) tags[i] = RemoveNewLinesAndTabs(tagNodes[i].InnerText);
        return tags;
    }

    private static string?[]? ExtractDevelopers(HtmlDocument document)
    {
        var developerNodes = document.DocumentNode.SelectSingleNode("//div[@id='developers_list']")?.ChildNodes;
        if (developerNodes == null) return null;

        string?[]? developers = new string[developerNodes.Count];
        for (var i = 0; i < developerNodes.Count; i++)
            developers[i] = RemoveNewLinesAndTabs(developerNodes[i].InnerText);
        return developers;
    }

    private static string? ExtractReleaseDate(HtmlDocument document)
    {
        var releaseDateNode = document.DocumentNode.SelectSingleNode("//div[@class='grid_content grid_date']");
        return releaseDateNode != null ? RemoveNewLinesAndTabs(releaseDateNode.InnerText) : null;
    }

    private static string? ExtractRecentReviewScore(HtmlDocument document)
    {
        var recentReviewScore = document.DocumentNode.SelectSingleNode("//div[@id='appReviewsRecent_responsive']");
        if (recentReviewScore == null) return null;
        
        var recentReviewScoreNode =
            recentReviewScore.SelectSingleNode("//span[@class='game_review_summary positive']") ??
            recentReviewScore.SelectSingleNode("//span[@class='game_review_summary negative']");
        return recentReviewScoreNode != null ? RemoveNewLinesAndTabs(recentReviewScoreNode.InnerText) : null;
    }

    private static string? ExtractAllTimeReviewScore(HtmlDocument document)
    {
        var allTimeReviewScoreNode = document.DocumentNode.SelectSingleNode("//div[@id='appReviewsAll_responsive']");
        if (allTimeReviewScoreNode == null) return null;
        
        var allReviewScoreNode =
            allTimeReviewScoreNode.SelectSingleNode("//span[@class='game_review_summary positive']") ??
            allTimeReviewScoreNode.SelectSingleNode("//span[@class='game_review_summary negative']");
        return allReviewScoreNode != null ? RemoveNewLinesAndTabs(allReviewScoreNode.InnerText) : null;
    }

    private static GameAreaDetail[]? ExtractGameAreaDetails(HtmlDocument document)
    {
        var gameAreaDetailsList = document.DocumentNode.SelectNodes("//div[@class='game_area_details_specs']");
        if (gameAreaDetailsList == null) return null;

        var gameAreaDetails = new GameAreaDetail[gameAreaDetailsList.Count];
        for (var i = 0; i < gameAreaDetailsList.Count; i++)
            gameAreaDetails[i] = new GameAreaDetail
            {
                Name =
                    RemoveNewLinesAndTabs(gameAreaDetailsList[i].SelectSingleNode(".//div[@class='name']").InnerText),
                Value = RemoveNewLinesAndTabs(gameAreaDetailsList[i].SelectSingleNode(".//div[@class='value']")
                    .InnerText)
            };
        return gameAreaDetails;
    }

    private static string? RemoveNewLinesAndTabs(string input)
    {
        return input.Replace("\t", string.Empty).Replace("\n", string.Empty);
    }
}