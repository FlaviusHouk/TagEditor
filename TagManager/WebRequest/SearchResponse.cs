using System.Collections.Generic;

namespace TagManager.WebRequest
{
	public class SearchResponse
	{
		public List<DisplayDoc> DisplayDocs { get; set; }
		public int NumberOfHits { get; set; }
	}
}
