// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code
		function setCookie(cname, cvalue, exdays) {
			const d = new Date();
			d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
			let expires = "expires=" + d.toUTCString();
			document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
		}

		// Delete cookie
		function deleteCookie(cname) {
			const d = new Date();
			d.setTime(d.getTime() + (24 * 60 * 60 * 1000));
			let expires = "expires=" + d.toUTCString();
			document.cookie = cname + "=;" + expires + ";path=/";
		}

		// Read cookie
		function getCookie(cname) {
			let name = cname + "=";
			let decodedCookie = decodeURIComponent(document.cookie);
			let ca = decodedCookie.split(';');
			for (let i = 0; i < ca.length; i++) {
				let c = ca[i];
				while (c.charAt(0) == ' ') {
					c = c.substring(1);
				}
				if (c.indexOf(name) == 0) {
					return c.substring(name.length, c.length);
				}
			}
			return "";
		}

		var cc = getCookie('user_cookie_consent');

if (cc == 1) { document.getElementById("cookieNotice").style.display = "none"; }
else { document.getElementById("cookieNotice").style.display = "block"; }

		// Set cookie consent
		function acceptCookieConsent() {
			deleteCookie('user_cookie_consent');
			setCookie('user_cookie_consent', 1, 30);
			document.getElementById("cookieNotice").style.display = "none";
		}
