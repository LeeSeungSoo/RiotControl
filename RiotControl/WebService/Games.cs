﻿using System.Collections.Generic;
using System.Web;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		string GetSummonerGamesTable(List<GameTeamPlayer> games)
		{
			string[] titles =
			{
				"Champion",
				"Map",
				"Mode",
				"Date",
				"K",
				"D",
				"A",
				"MK",
				"NK",
				"Gold",
				"Rating",
				"Items",
				"Premade",
				"Ping",
				"Time in queue",
			};
			string firstRow = "";
			foreach (var title in titles)
				firstRow += Markup.TableHead(title);
			string rows = Markup.TableRow(firstRow);
			foreach (var game in games)
			{
				string championName = GetChampionName(game.ChampionId);
				string championDescription = Markup.Image(GetImage(string.Format("Champion/Small/{0}.png", HttpUtility.UrlEncode(championName))), championName) + championName;
				string items = "\n";
				foreach (var itemId in game.Items)
				{
					if (itemId == 0)
						items += Markup.Image(GetImage("Item/Small/Blank.png"), "Unused");
					else
					{
						ItemInformation item = GetItemInformation(itemId);
						items += Markup.Image(GetImage(string.Format("Item/Small/{0}.png", itemId)), item.Name);
					}
				}
				string ratingDescription = "";
				if (game.Rating.HasValue && game.Rating > 0)
					ratingDescription = string.Format("{0} ({1})", game.Rating + game.RatingChange, SignumString(game.RatingChange.Value));
				string[] fields1 =
				{
					championDescription,
					game.Map.GetString(),
					game.GameMode.GetString(),
					game.GameTime.ToString(),
					game.Kills.ToString(),
					game.Deaths.ToString(),
					game.Assists.ToString(),
					game.MinionKills.ToString(),
					game.NeutralMinionsKilled.HasValue ? game.NeutralMinionsKilled.ToString() : "-",
					game.Gold.ToString(),
					ratingDescription,
				};
				string row = "";
				foreach (var field in fields1)
					row += Markup.TableCell(field);
				row += Markup.ContentTag("td", items, new Dictionary<string, string>() {{"class", "items"}}, true);
				string[] fields2 =
				{
					game.PremadeSize == 1 ? "No" : string.Format("Yes, {0}", game.PremadeSize),
					string.Format("{0} ms", game.Ping),
					string.Format("{0} s", game.TimeSpentInQueue),
				};
				foreach (var field in fields2)
					row += Markup.TableCell(field);
				rows += Markup.TableRow(row, style: game.Won ? "win" : "loss");
			}
			string table = Markup.Table(rows, style: "statistics", id: "summonerGames");
			return table;
		}
	}
}