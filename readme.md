# REffffound

its ffffound - lomng lost, now refound :)


## v .1 ok
- *RE:ffffound logo > Y*
- view Feed:, mindestens 2 seitig (20stück), keine kontextbilder(?) > Y
	- link 'saved by' user auf /bookmarks/guid > Y
- *View Detail: einzelbild, keine kontextbilder (rechts), keine kontextuser > Y*
- *View: Aktive Nutzer,  hardkodierten > Y*
- *Feeddaten, Userdaten hardkodiert > Y*
- *kontextmenus: log/register mit pop up > Y*
- *Bonus: Userfeeds > Y*


## v .2 ==ok==

* Testhosting, ng version
 
![alt text](Documentation/20240521175852.png)

## v .25
* ~~Unitttests für Models (zB. Timestamp parsing) > Y/2: mit TestController~~ 
* ~~CRUDLIST fähig für Posts > 3/4 >> Update/Put NOK issue '#6/#?/#9'> Y~~
* ~~Feeddaten, userdaten hardkodiert > Y~~
* ~~Guid bei post erzeugt #8 > Y~~

## v .30 ==ok==
* ~~angular version: use observables > Y~~

## v .xy ng Version nok
* ~~Frontend an Api angebunden > Y~~
	* ~~https://www.npmjs.com/package/ts-sync-request~~
	* ~~https://stackoverflow.com/questions/62296092/how-to-make-synchronous-http-request-in-angular-8-or-9-make-a-request-and-wait~~
* ~~Daten aus Api hardkodiert~~
* ~~CORS Fehler korrigiert > N~~ 
*  api gehostet

## v .3 ==ok==
* ~~Api an DB (Mongo? > SQL) angebunden~~
* ~~Daten read an FE~~

## v .4 
* ~~Daten write aus Front End: Posts~~
*  ~~Index Feed inkl Kontextbilder~~
* Create Bookmark mit username via ViewBags
* ~~incl header logo bmp~~
* fix css 
  * ~~a:color~~
  * ~~font-type > serifiger~~
  * differenz blockquote position index & list
  * link zu Flag Image more subtle
* Bookmark-element im feed: add 'Quoted from', link to url
* ~~Fix timestamp bug where current minutes saved as month~~
* alle queries mit .AddParameterWithValue > kein stringgebaue 
* Hydrate() -> testdaten Create()..

## v .5
* partial views (activeUsers..) füllen
  * fill dbo.ContentUsers with testusers
  * on Bookmarks/Create increase Count property for posting Users by 1 
* dynamische pagination
* in indexfeed
* in userfeed

## v .6
* in 'Bookmarks/Create' fill Title Field automaticlly > UrlFfield.onChange() request 
* Load spinner for images

## v .7
* View Detail inkl Kontextbilder
* View Detail inkl Kontextuser

## v .x
* self hosting Images > azure blobs? 
* User Auth
  * sicherung > simpler api key oder jwt?
	* apikey: festerkeyhardcorediert wird bei jedem req in die payload gelgt und im controller abgefragt
	* jwt mit .net core:
	* https://learn.microsoft.com/de-de/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-8.0

## v .y
* Save bookmark for logged in users
