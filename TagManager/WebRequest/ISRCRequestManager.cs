using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace WebRequests
{
	public class ISRCRequestManager
    {
		private static ISRCRequestManager _inst = new ISRCRequestManager();
		private Uri uri = new Uri("https://isrcsearch.ifpi.org");
		private Uri searchUri = new Uri("https://isrcsearch.ifpi.org/api/v1/search");

		private HttpClient _client;

		public static ISRCRequestManager Inst
		{
			get
			{
				return _inst;
			}
		}

		public ISRCRequestManager()
		{
			CookieContainer cookieContainer = new CookieContainer();
			HttpClientHandler hand = new HttpClientHandler() { CookieContainer = cookieContainer };

			_client = new HttpClient(hand);
			_client.BaseAddress = uri;


			var rm = _client.GetAsync(uri).Result;
			var co = cookieContainer.GetCookies(uri).Cast<Cookie>();

			_client.DefaultRequestHeaders.TryAddWithoutValidation("X-CSRFToken", $"{co.FirstOrDefault().Value}");
			_client.DefaultRequestHeaders.Referrer = uri;
		}

		public SearchResponse GetFromData(string artName, string trackName, int year = 0)
		{
			string myObject = "{searchFields:{artistName:\""+artName+"\",trackTitle:\""+trackName+ "\",releaseYear:\"" + (year <= 0 ? string.Empty : year.ToString())+ "\"}, showReleases:0,start:0,number:10}";
			JObject jo = JObject.Parse(myObject);
			var query = new StringContent(jo.ToString(), Encoding.UTF8, "application/json");
			var res = _client.PostAsync(searchUri, query);
			string resAsString = res.Result.Content.ReadAsStringAsync().Result;
			SearchResponse response = JsonConvert.DeserializeObject<SearchResponse>(resAsString);
			return response;
		}

		public SearchResponse GetFromISRC(string ISRC)
		{ 
			var myISRC = "{searchFields: {isrcCode:\""+ ISRC +"\"}, showReleases: true, start: 0, number:10}";
			var jo = JObject.Parse(myISRC);
			var query = new StringContent(jo.ToString(), Encoding.UTF8, "application/json");
			var res = _client.PostAsync(searchUri, query);
			var resAsString = res.Result.Content.ReadAsStringAsync().Result;
			SearchResponse response = JsonConvert.DeserializeObject<SearchResponse>(resAsString);
			return response;
		}

	}
}
