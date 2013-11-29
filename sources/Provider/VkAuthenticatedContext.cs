using System;
using System.Globalization;
using System.Security.Claims;
using System.Xml;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace Duke.Owin.VkontakteMiddleware.Provider
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class VkAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="VkAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="userxml">The XML document with user info</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="expires">Seconds until expiration</param>
        public VkAuthenticatedContext(IOwinContext context, XmlDocument userxml, string accessToken, string expires)
            : base(context)
        {
            UserXml = userxml;
            AccessToken = accessToken;

            int expiresValue;
            if (Int32.TryParse(expires, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue))
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
            }

			Id = TryGetPropertyValue("uid");
			Name = TryGetPropertyValue("first_name");
			LastName = TryGetPropertyValue("last_name");
			UserName = TryGetPropertyValue("screen_name");
			Nickname = TryGetPropertyValue("nickname");
			Link = TryGetPropertyValue("photo_50");

        }

        /// <summary>
        /// Gets the document with user info
        /// </summary>
        public XmlDocument UserXml { get; private set; }

        /// <summary>
        /// Gets the access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; set; }

        /// <summary>
        /// Gets the user ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the user's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the user's last name
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the user's full name
        /// </summary>
        public string FullName
        {
            get
            {
                return Name + " " + LastName;
            }
        }


        /// <summary>
        /// Gets the user's DefaultName
        /// </summary>
        public string DefaultName
        {
            get
            {
                if (!String.IsNullOrEmpty(UserName))
                    return UserName;

                if (!String.IsNullOrEmpty(Nickname))
                    return Nickname;

                return FullName;
            }
        }

        /// <summary>
        /// Gets the user's picture link
        /// </summary>
        public string Link { get; private set; }

        /// <summary>
        /// Gets the username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the Nickname
        /// </summary>
        public string Nickname { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

		/// <summary>
		/// Returns value of the property with the specified name in exist
		/// </summary>
		/// <param name="propertyName">The name of the property</param>
		/// <returns>Value of the property</returns>
		public string TryGetPropertyValue(string propertyName)
        {
            XmlNodeList elemList = UserXml.GetElementsByTagName(propertyName);
            if (elemList != null)
            {
                if (elemList[0] != null)
                    return elemList[0].InnerText.Trim();
            }

            return String.Empty;
        }
    }
}
