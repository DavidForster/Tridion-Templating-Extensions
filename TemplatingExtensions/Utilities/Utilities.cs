using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Templating;

namespace TemplatingExtensions.Utilities
{
    /* TODO: Clean this class */
	public class Utilities
	{
        public static string GetWebDavUrl(Publication pub, string relUrl)
        {
            return GetWebDavUrl(pub.Title, relUrl);
        }

        public static string GetWebDavUrl(string publicationTitle, string relUrl)
        {
            if (!relUrl.StartsWith("/"))
            {
                relUrl = String.Concat("/", relUrl);
            }

            string webDavUrl = Uri.UnescapeDataString(String.Concat("/webdav/", publicationTitle, relUrl));

            return webDavUrl;
        }

		/// <summary>
		/// Return a list of objects of the requested type from the XML.
		/// </summary>
		/// <remarks>
		/// This method goes back to the database to retrieve more information. So it is NOT just
		/// a fast and convenient way to get a type safe list from the XML.
		/// </remarks>
		/// <typeparam name="T">The type of object to return, like Publication, User, Group, OrganizationalItem</typeparam>
        /// <param name="engine">TOM.Net Engine</param>
		/// <param name="listElement">The XML from which to construct the list of objects</param>
		/// <returns>a list of objects of the requested type from the XML</returns>
		protected IList<T> GetObjectsFromXmlList<T>(Engine engine, XmlElement listElement) where T : IdentifiableObject
		{
			XmlNodeList itemElements = listElement.SelectNodes("*");
			List<T> result = new List<T>(itemElements.Count);
		    result.AddRange(from XmlElement itemElement in itemElements select GetObjectFromXmlElement<T>(engine, itemElement));
		    result.Sort((item1, item2) => item1.Title.CompareTo(item2.Title));
			return result;
		}

		protected T GetObjectFromXmlElement<T>(Engine engine, XmlElement itemElement) where T : IdentifiableObject
		{
			return (T)engine.GetObject(itemElement.GetAttribute("ID"));
		}


        public static string GetMetadataStringValue(XmlElement metadata, string fieldName)
        {
            string fieldValue = string.Empty;
            if (metadata != null && metadata[fieldName] != null)
            {
                fieldValue = metadata[fieldName].InnerText;
            }
            return fieldValue;
        }

		/// <summary>
		/// Converts a string to a memory stream
		/// </summary>
		public static Stream ConvertStringToStream(string str)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(str));
		}

        public static string GetFilename(string fullpath)
        {
            if (fullpath.Contains(@"\"))
            {
                int pos = fullpath.LastIndexOf(@"\");
                return fullpath.Substring(pos + 1);
            }
            return fullpath;
        }

        public static string RemoveWhitespaces(string value)
        {
            return (!String.IsNullOrEmpty(value)) ? Regex.Replace(value, @"\s", String.Empty) : String.Empty;
        }

		public static string StripPrefix(string title)
		{
		    if (title.IndexOf(" ") < 0)
            {
                return title;
            }
            
            string prefix = title.Substring(0, title.IndexOf(" "));          
			
			int result;
			char[] space = { ' ' };
			//Handle number prefix
			int.TryParse(prefix, out result);
			if (result != 0)
			{
				title = title.Substring(title.IndexOf(" ") + 1);
			}
			//Handle double colon prefix
			if (title.Contains("::"))
			{
				title = title.Remove(0, title.IndexOf("::") + 2);
			}
			title = title.TrimStart(space);
			return title;
		}

        public static bool IsWebDavUrl(string url)
        {
            return !string.IsNullOrEmpty(url)
                && url.ToLower().StartsWith("/webdav/");
        }

        public static List<string> TrimWhiteSpaces(IEnumerable<string> stringArray)
        {
            return stringArray.Select(s => s.Trim()).ToList();
        }


	}
}
