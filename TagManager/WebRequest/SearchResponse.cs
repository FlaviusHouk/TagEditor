using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRequests
{
	public class SearchResponse
	{
		public List<DisplayDoc> DisplayDocs { get; set; }
		public int NumberOfHits { get; set; }
	}
}
