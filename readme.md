# REffffound

its ffffound - long lost, now refound :)

## Backlog items

### v .1 ok
- *RE:ffffound logo > Y*
- view Feed:, mindestens 2 seitig (20stück), keine kontextbilder(?) > Y
	- link 'saved by' user auf /bookmarks/guid > Y
- *View Detail: einzelbild, keine kontextbilder (rechts), keine kontextuser > Y*
- *View: Aktive Nutzer,  hardkodierten > Y*
- *Feeddaten, Userdaten hardkodiert > Y*
- *kontextmenus: log/register mit pop up > Y*
- *Bonus: Userfeeds > Y*


### v .2 ==ok==

* Testhosting, ng version
 
![alt text](Documentation/20240521175852.png)

### v .25
* ~~Unitttests für Models (zB. Timestamp parsing) > Y/2: mit TestController~~ 
* ~~CRUDLIST fähig für Posts > 3/4 >> Update/Put NOK issue '#6/#?/#9'> Y~~
* ~~Feeddaten, userdaten hardkodiert > Y~~
* ~~Guid bei post erzeugt #8 > Y~~

### v .30 ==ok==
* ~~angular version: use observables > Y~~

### v .xy ng Version nok
* ~~Frontend an Api angebunden > Y~~
	* ~~https://www.npmjs.com/package/ts-sync-request~~
	* ~~https://stackoverflow.com/questions/62296092/how-to-make-synchronous-http-request-in-angular-8-or-9-make-a-request-and-wait~~
* ~~Daten aus Api hardkodiert~~
* ~~CORS Fehler korrigiert > N~~ 
*  api gehostet

### v .3 ==ok==
* ~~Api an DB (Mongo? > SQL) angebunden~~
* ~~Daten read an FE~~

### v .4 ==ok==
* ~~Daten write aus Front End: Posts~~
*  ~~Index Feed inkl Kontextbilder~~
* ~~Create Bookmark mit username via ViewBags~~
* ~~incl header logo bmp~~
* ~~fix css~~
  * ~~a:color~~
  * ~~font-type > serifiger~~
  * ~~link zu Flag Image more subtle~~
  * ~~fix blockquote position~~
  * ~~fix container position to left~~
* ~~text: fix title~~
* ~~Bookmark-element im feed: add 'Quoted from', link to url~~
* ~~Fix timestamp bug where current minutes saved as month~~
* ~~Hydrate() -> testdaten Create()..~~
* ~~Hydrate()~~
  * ~~generate timestamp from wihtin last 3 months and overwrite timestamps~~

 
### v .5

* ~~connection string & credentials in appsettings.json~~
* ~~feed navigation: previous- & next- buttons without 'full-pagination'~~
 * ~~include 'Last Post' logic to mark the end of feed~~
 * ~~add variety to Last Post to create a funny moment and communicate my crafty intentions~~ 
* ~~update end-of-feed View as own errror view. incl links to prev/next feed pages~~
* ~~css: difference in blockquote position index & list~~
* ~~css: fix txt position for current pagenum in pagination~~
* ~~all Database - queries mit .AddParameterWithValue > kein stringgebaue~~
* ~~Fix Bug in 'Update()' Method where bookmark.Title is being overwritten with Value from bookmark.Guid*~~
 
### v .6

* ~~add partial pages for Context menu, Legal menu~~
* ~~fix sql command string bug in List statement~~
* ~~Add Views and Functionality~~
 * ~~Edit bookmark~~
 * ~~Delete Bookmark~~
 * ~~Create Bookmark and Validate Input in Data Model class~~
* ~~remove View 'Old List'~~
* ~~fix paging with negative number for Index, List, 404~~
* ~~static float menu~~
 * ~~feed (index) for all: tile(v) [non actionable], top(t), previous(p), next (n)~~
 * ~~list (list) for user: create (c), tile(v), top(t), previous(p), next (n)~~
 * ~~detail: edit (e), delete (d)~~
 * ~~for Detail View: add dynamic menu item 'Back to list' linking to indexfeed or to userlist~~
 * ~~adminpage: all from List page plus hydrate(h)~~
 * ~~fix broken link: 404 page to feed~~
 * ~~dont display guid on edit/delete pages~~ 
 * ~~Use stronger Action text on Edit/Delete inputs for better Clarity of Actions~~

### v .7

block links:
- [x] register: ".. is invitation based service. If you like to receive an invitation you can contact us"
- [x] login: ".. if you have received an invite, please use the direct link provided." 
- [x] css cleanup for menu items 
- [x] for Release/Production Environment or without login hide the followingFunctions: Create, Edit, Delete, Hydrate
- [ ] Add validation Message in Create View for invalid inputs

### v 0.8 - 0.9

* partial views (activeUsers..) füllen
  * fill dbo.ContentUsers with testusers
  * on Bookmarks/Create increase Count property for posting Users by 
* in 'Bookmarks/Create' fill Title Field automaticlly > UrlField.onChange() request 
* Load spinner for images
* View Detail inkl Kontextbilder
 * 10 posts (5x2)
* View Detail inkl Kontextuser
 * die letzten 5 (5x1) Posts
* dynamische pagination: in indexfeed, in userfeed

## v x.y
* On users feed page list in sidebar
  * pageusers favourite Top10 users > users who pageuser favourited the most
  * pageusers 10 most recent followers (first establish basic pub>sub model)
* self hosting Images > azure blobs? 
* User Auth
  * sicherung > simpler api key oder jwt?
	* apikey: festerkeyhardcorediert wird bei jedem req in die payload gelgt und im controller abgefragt
	* jwt mit .net core:
	* https://learn.microsoft.com/de-de/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-8.0
* Save bookmark for logged in users
* 'save' functionality for bookmarks of other Users

