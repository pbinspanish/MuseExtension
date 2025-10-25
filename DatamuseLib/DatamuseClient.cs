namespace DatamuseLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

public class DatamuseDefinition
{
	public string PartOfSpeech { get; set; }
	public string Text { get; set; }
}

public class DatamuseWordResult
{
	public string Word { get; set; }
	public int Score { get; set; }
	public List<DatamuseDefinition> Definitions { get; set; } = new List<DatamuseDefinition>();
}

public class DatamuseClient
{
	private static readonly HttpClient httpClient = new HttpClient();

	public DatamuseWordResult[] GetWordInfo(string word)
	{
		if (string.IsNullOrWhiteSpace(word))
			return Array.Empty<DatamuseWordResult>();

		try
		{
			string url = $"https://api.datamuse.com/words?sp={Uri.EscapeDataString(word)}&md=d";
			var response = httpClient.GetStringAsync(url).GetAwaiter().GetResult();

			using var doc = JsonDocument.Parse(response);
			var results = new List<DatamuseWordResult>();

			foreach (var item in doc.RootElement.EnumerateArray())
			{
				var result = new DatamuseWordResult
				{
					Word = item.TryGetProperty("word", out var w) ? w.GetString() : "",
					Score = item.TryGetProperty("score", out var s) ? s.GetInt32() : 0
				};

				if (item.TryGetProperty("defs", out var defsArray))
				{
					foreach (var def in defsArray.EnumerateArray())
					{
						var raw = def.GetString() ?? "";
						var parts = raw.Split('\t', 2);
						string pos = parts.Length > 1 ? ExpandPartOfSpeech(parts[0].Trim()) : "";
						string text = parts.Length > 1 ? parts[1].Trim() : raw.Trim();

						result.Definitions.Add(new DatamuseDefinition
						{ 
							PartOfSpeech = pos,
							Text = text
						});
					}
				}

				results.Add(result);
			}

			return results.ToArray();
		}
		catch (HttpRequestException)
		{
			Console.WriteLine("Network error: unable to reach Datamuse API.");
			return Array.Empty<DatamuseWordResult>();
		}
		catch (JsonException)
		{
			Console.WriteLine("Error parsing API response.");
			return Array.Empty<DatamuseWordResult>();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Unexpected error: {ex.Message}");
			return Array.Empty<DatamuseWordResult>();
		}
	}

	private static string ExpandPartOfSpeech(string abbr) => abbr switch
	{
		"n" => "noun",
		"v" => "verb",
		"adj" => "adjective",
		"adv" => "adverb",
		_ => abbr
	};
}
