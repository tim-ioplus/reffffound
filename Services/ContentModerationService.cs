using reffffound.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace reffffound.Services
{
    public class ContentModerationService
    {
		public ContentModerationService() { }

		public bool AreUrlsValid(Bookmark bookmark)
		{
			bool isValid = false;
			var validationMessage = string.Empty;

			isValid = IsUrlValid(bookmark.Url.ToLower()) && IsUrlValid(bookmark.Image.ToLower());

			if(isValid)
			{
				validationMessage = "Provided links are valid.";
			}
			else
			{
				validationMessage = "Please do not user links from " + "facebook"  + " domains.";
			}

			return isValid;
		}
		public bool IsUrlValid(string urlToCheck)
		{
			var invalidurls = new List<string>()
			{
				"facebook.com","facebook.net","fbcdn.net","fbcdn.com","fb.com","fbcdn-profile-a.akamaihd.net",
				"spot.im", "/cdn-cgi/pe/bag2?*connect.facebook.com", "/cdn-cgi/pe/bag2?*connect.facebook.net"
			};

			var urlInvalid = string.IsNullOrWhiteSpace(urlToCheck) || invalidurls.Any(u => urlToCheck.Contains(u));

			bool urlValid = !urlInvalid;
			return urlValid;
		}
    }
}
