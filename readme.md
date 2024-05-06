 reffffound

its ffffound - long lost, now refound :-)


1. /frontend
    1. Masterview: index/homefeed: 
        1. header: reffffound logo, claim
        2. sidebar: Links zu.. 
            1. About, Anmelden, Screensaver, IPhone, -register-
            2. Privacy, TToS, Contact, Change Logg, Feed (rss url)
            3. zuletzt aktice Nutzer  inkl Postanzahl: _nutzername (anzahl posts)_
        1. feedbar
            1. 25 posts per Page
            2. Post Feedview Contains: Quoted Url, Image, Link zu Post, Preview Images of 3 Adjacent Posts
    2. Detailview: /image/{id}
        1. Quoted Url: Titel, Link
        2. Image, max 520px breit, höhe auto
        3. Link zu Flagging: repost Posts for Lagalities etc.
        4. 'postred on:' Time-Stamp 
        5. Other users saving this Image
        6. recommendation posts: preview images of ~10 other posted images by user
        7. recommendation Users: intro to ~10 other users, with: 
            1. link to user
            2. preview Images of ~5 selected posts of user linking to respective post
    3. Create
        1. Enter Image url
        2. Enter Page url (when cannot be parsed from image url!)
        3. Credentials (use token as long as user mangement not ready)
    4. Register: enter name, email, pw
    5. Login
2. /backend
    1. posts api: crud, list
    2. users api: crud, login 
    3. img API: (later on)
