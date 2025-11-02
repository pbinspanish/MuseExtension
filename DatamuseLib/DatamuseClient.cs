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


public struct DatamuseSuggestion
{
	public string Word { get; set; }
	public int Score { get; set; }
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

	public static DatamuseSuggestion[] GetSuggestions(string text)
	{
		// if there's no input, there can be no suggestions
		if (string.IsNullOrWhiteSpace(text)) return Array.Empty<DatamuseSuggestion>();

		try
		{
			string url = $"https://api.datamuse.com/sug?s={Uri.EscapeDataString(text)}";
			var response = httpClient.GetStringAsync(url).GetAwaiter().GetResult();

			using var doc = JsonDocument.Parse(response);
			var results = new List<DatamuseSuggestion>();

			foreach (var item in doc.RootElement.EnumerateArray())
			{
				var result = new DatamuseSuggestion
				{
					Word = item.TryGetProperty("word", out JsonElement w) ? w.GetString() ?? "" : "",
					Score = item.TryGetProperty("score", out JsonElement s) ? s.GetInt32() : 0
				};

				results.Add(result);
			}

			return results.ToArray();
		}
		catch (HttpRequestException)
		{
			Console.WriteLine("Network error: unable to reach Datamuse API.");
			return Array.Empty<DatamuseSuggestion>();
		}
		catch (JsonException)
		{
			Console.WriteLine("Error parsing API response.");
			return Array.Empty<DatamuseSuggestion>();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Unexpected error: {ex.Message}");
			return Array.Empty<DatamuseSuggestion>();
		}
	}

	public DatamuseWordResult[] GetWordInfo(string word)
	{
		// constraints:
		//   means like
		//   sounds like
		//   spelled like
		//   related words:
		//     jja
		//     jjb
		//     synonyms
		//     triggers
		//     antonyms
		//     kind of (hypernyms)
		//     more general than (hyponyms)
		//     comprises (holonyms)
		//     part of (meronyms)
		//     frequent followers
		//     frequent predecessors
		//     homophones
		//     consonant match

		// vocabularies
		//   default (english)
		//   spanish
		//   wikipedia en

		// hints:
		//   topics
		//   left context
		//   right context

		// results:
		//   maximum results
		//   metadata:
		//     definitions
		//     parts of speech
		//     syllable count
		//     pronunciation
		//     word frequency


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
		"u" => "unknown",
		_ => abbr
	};
}
