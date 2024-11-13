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


	 
## Backlog items

### v .1 ok

* * view Feed:, mindestens 2 seitig (20stück), keine kontextbilder(!) [x]
	* * link 'saved by' user auf /bookmarks/guid [x]
* * View Detail: einzelbild, keine kontextbilder (rechts), keine kontextuser > [x]
* * View: Aktive Nutzer,  hardkodierten [x]
* * Feeddaten, Userdaten hardkodiert [x]
* * Userfeeds [x]
* * RE:ffffound logo [x]


### v .2 ==ok==

* Testhosting, ng version
 
![alt text](Documentation/20240521175852.png)

### v .25
* * Unitttests für Models (zB. Timestamp parsing) > Y/2: mit TestController [x]  
* * CRUDLIST fähig für Posts > 3/4 >> Update/Put NOK issue '#6/#?/#9'> Y [x] 
* * Feeddaten, userdaten hardkodiert > Y [x] 
* * Guid bei post erzeugt #8 > Y [x] 

### v .30 ==ok==
* * angular version: use observables > Y [x] 

### v .xy ng Version nok
* * Frontend an Api angebunden > Y [x] 
	* * https://www.npmjs.com/package/ts-sync-request [x] 
	* * https://stackoverflow.com/questions/62296092/how-to-make-synchronous-http-request-in-angular-8-or-9-make-a-request-and-wait [x] 
* * Daten aus Api hardkodiert [x] 
* * CORS Fehler korrigiert > N [x]  
*  api gehostet

### v .3 ==ok==
* * Api an DB (Mongo? > SQL) angebunden [x] 
* * Daten read an FE [x] 

### v .4 ==ok==
* * Daten write aus Front End: Posts [x] 
* * Index Feed inkl Kontextbilder [x] 
* * Create Bookmark mit username via ViewBags [x] 
* * incl header logo bmp [x] 
* * fix css [x] 
  * * a:color [x] 
  * * font-type > serifiger [x] 
  * * link zu Flag Image more subtle [x] 
  * * fix blockquote position [x] 
  * * fix container position to left [x] 
* * text: fix title [x] 
* * Bookmark-element im feed: add 'Quoted from', link to url [x] 
* * Fix timestamp bug where current minutes saved as month [x] 
* * Hydrate() -> testdaten Create().. [x] 
* * Hydrate() [x] 
  * * generate timestamp from wihtin last 3 months and overwrite timestamps [x] 

 
### v .5

* * connection string & credentials in appsettings.json [x] 
* * feed navigation: previous- & next- buttons without 'full-pagination' [x] 
 * * include 'Last Post' logic to mark the end of feed [x] 
 * * add variety to Last Post to create a funny moment and communicate my crafty intentions [x]  
* * update end-of-feed View as own errror view. incl links to prev/next feed pages [x] 
* * css: difference in blockquote position index & list [x] 
* * css: fix txt position for current pagenum in pagination [x] 
* * all Database - queries mit .AddParameterWithValue > kein stringgebaue [x] 
* * Fix Bug in 'Update()' Method where bookmark.Title is being overwritten with Value from bookmark.Guid* [x] 
 
### v .6-0.7

* * add partial pages for Context menu, Legal menu [x] 
* * fix sql command string bug in List statement [x] 
* * Add Views and Functionality [x] 
  * * Edit bookmark [x] 
  * * Delete Bookmark [x] 
  * * Create Bookmark and Validate Input in Data Model class [x] 
* * remove unused Backup View 'OldList' [x] 
* * fix paging with negative number for Index, List, 404 [x] 
* * static float menu [x] 
  * * feed (index) for all: tile(v) [non actionable], top(t), previous(p), next (n) [x] 
  * * list (list) for user: create (c), tile(v), top(t), previous(p), next (n) [x] 
  * * detail: edit (e), delete (d) [x] 
  * * for Detail View: add dynamic menu item 'Back to list' linking to indexfeed or to userlist [x] 
* * adminpage: all from List page plus hydrate(h) [x] 
* * fix broken link: 404 page to feed [x] 
* * dont display guid on edit/delete pages [x]  
* * Use stronger Action text on Edit/Delete inputs for better Clarity of Actions [x] 

### v .8 ==ok==
* * block links
  * * register: ".. is invitation based service. If you like to receive an invitation you can contact us" [x]
  * * login: ".. if you have received an invite, please use the direct link provided." [x]
* * css cleanup for menu items [x]
* * for Release/Production Environment or without login hide the following Functions: Create, Edit, Delete, Hydrate [x] 
* * Add validation Message in Create- and Edit-View for invalid inputs [x]
* * Create and use Helper Service für User/Identity Tasks [x]


### v 0.9 - 0.91

*  * fix style for pagination links, style for 'First' link dynamic [x] 
*  * on 404 page move 'Back to List/feed' Link to bottom in pagination area [x] 
*  * move app hosting to azure [x]
*  * move database hosting to azure [x] 
*  * fix sql parameter bug in Statement to Select Contextual Bookmarks [x]
*  * add temporary aws downtime Message [x]
*  * remove version log [x]


### v 0.92

*  * dynamic filling of partial View for User Activity [x]
   
### v 1.0

* * encode email adress on pages: contact, flag content [x]
* * update content Imprint, Terms of Service [x]
* * fix layout on About page [x]
* * show cookie Banner [x]
* * remove aws Downtime Message [x]

### v 1.1

* fix recommendation links [x]
  * recommendation appear on sidebar in global (index) and user (List) feeds
  * 0 <= n <= 3 recommmendations per post
  * recommendations predate the recommending post
  * each post only recommends from its own user
  * no repeatitions on current page per user
  * recommendations are not sorted


### v 1.2

* partial views
  * on bookmark Create increase Count property for posting Users  
* on admin functions hydrate, UpdateUsers return to Users feed noch index feed

### v1.3

* dynamische pagination: in indexfeed, in userfeed [x]
* 'As a user navigating the bookmarks, i'd' like to have more Information about the site's content size so its easier for me to know where i am and where to go next.' [x]
  * Show links to navigate forward and backward in content
  * Show current oage number amog all page numbers
  * move pagination to own Component
* 'As an user i'd like to see who are the recent posting users an how much they are posting to give me an idea of the activity on the platform' [x]
  * show user name sand post count on sidebar
  * move User-View into own Compoenent
 
### v 1.4 

* update 'About' page with screenshot of original fffffound [x]
* on creating Bookmark, parse Posting title from Url (alpha) [x]

### v 1.5

* save backup data from azure [x]
* cleanup data [x]
* restore data locally [x]
* create Hydrate view to upload bulkfile for Bookmarks [x]
   * Insert bookmark if guid not posted yet
	* Update POsting count for bokmarks users


### v 1.6

* on Detail View incl recommendations
 * from posting User: 10 posts (selection method: last 5 plus 5 random)
* fix double guid parameter on within Links on 'List'-pages

 
### v 1.7

* User Auth
* 'found functionality': Save bookmark of other users for logged in users
* switch to local sqlite database
* adding docker containerizing capabilities
* on Detail View of Bookmarks 
 * add recommendations: links to posts from refounding Users 
 * selection: from at most 10 reposting Users: select at most 5 posts (last 5) to link to beneathe main Post
* include archive.org ddos message for bitmaps linked to archive.org wayback machine
* fix bug in bookmark deletion

  
### v x.y

 
* fix display of validation message on bookmark Create
* in 'Bookmarks/Create' fill Title Field automaticlly > UrlField.onChange() request 
* Load spinner for images
  * On users feed page list in sidebar
  * pageusers favourite Top10 users > users who pageuser favourited the most
  * pageusers 10 most recent followers (first establish basic pub>sub model)
* self hosting Images > azure blobs? 
* 'save' functionality for bookmarks of other Users
* move all bitmaps to hosting
* move all bitmaps to cdn

