# REffffound

its ffffound - lomng lost, now refound :)


## v 1..
- *RE:ffffound logo > Y*
- *View Feed:, mindestens 2 seitig (20stück), keine kontextbilder(?) > Y*
	- *link 'saved by' user auf /image/:guid > Y*
- *View Detail: einzelbild, keine kontextbilder (rechts), keine kontextuser > Y*
- *View: Aktive Nutzer,  hardkodierten > Y*
- *Feeddaten, Userdaten hardkodiert > Y*
- *kontextmenus: log/register mit pop up > Y*
- *Bonus: Userfeeds > Y*


## v 1+..

* Testhosting, ng version
 
![alt text](Documentation/20240521175852.png)

v..
* ~~Unitttests für Models (zB. Timestamp parsing) > Y/2: mit TestController~~ 
* ~~CRUDLIST fähig für Posts > 3/4 >> Update/Put NOK issue '#6/#?/#9'> Y~~
* ~~Feeddaten, userdaten hardkodiert > Y~~
* Guid bei post erzeugt #8 > Y



v..
* use observables > Y

v..
* ==Frontend an Api angebunden== 
	* ==https://www.npmjs.com/package/ts-sync-request==
	* ==https://stackoverflow.com/questions/62296092/how-to-make-synchronous-http-request-in-angular-8-or-9-make-a-request-and-wait==
* ==Daten aus Api hardkodiert==
* ==CORS Fehler korrigiert== 
	d* >> how to host/deploy net api?  (any api?)
*  ==api gehostet==

v.. 
* Api an DB (Mongo?) angebunden
* Daten read an FE

v..
* User Auth
  * **sicherung > simpler api key oder jwt?** >> ?
	* apikey: festerkeyhardcorediert wird bei jedem req in die payload gelgt und im controller abgefragt
	* jwt mit .net core:
	* https://learn.microsoft.com/de-de/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-8.0

v..
* ==Daten write aus Front End: Posts==
*  ==View Feed inkl Kontextbilder==
* allq queries mit .AddPArameterWithValue >> kein stringgebaue 
* testdaten Create()..

v..
* dynamische pagination
* 

v..
* Pagination in userfeed

v..
* Load spinner for images
* View Detail inkl Kontextbilder
* View Detail inkl Kontextuser
* self hosting Images > azure blobs? 
* 

