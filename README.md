# movions-backend
The backend/API for movions

# Available API calls:
## Company
### Create company:
Request:                                    | Response - created: | Response - bad request: | 
------------------------------------------- | ------------------- | ----------------------  |
expects: ```{ name: string, type: int }```  | httpcode: 201 ✔️    | httpcode: 400 ❌
method: POST                                | returns: company    | returns: errors
url: /api/company                           |

### Connect movie:
Request:                                    | Response - ok:      | Response - not found: | 
------------------------------- | ------------------- | --------------------- |
expects: ```{ movieId: int }``` | httpcode: 200 ✔️    | httpcode: 404 ❌
method: POST                    | returns: company    | returns: errors
url: /api/company/{id}/movies   |

### Get company:
Request:               | Response - ok:      | Response - not found: | 
---------------------- | ------------------- | --------------------- |
method: GET            | httpcode: 200 ✔️    | httpcode: 404 ❌
url: /api/company/{id} | returns: company    | returns: nothing


### Get companies:
Request:          | Response - ok:             | Response - no content: | 
----------------- | -------------------------- | ---------------------- |
method: GET       | httpcode: 200 ✔️           | httpcode: 204 ✔️
url: /api/company | returns: list of companies | returns: nothing

### Update company:
Request:                                            | Response - ok: | Response - bad request: | Response - not found:
--------------------------------------------------- | ------------------- | ------------------ | --------------------- |
expects: ```{ id: int, name: string, type: int }``` | httpcode: 20 ✔️    | httpcode: 400 ❌   | httpcode: 404 ❌
method: PUT                                         | returns: company    | returns: errors    | returns: nothing
url: /api/company/{id}                              |

### Delete company:
Request:               | Response - ok:      | Response - not found: | 
---------------------- | ------------------- | --------------------- |
method: DELETE         | httpcode: 200 ✔️    | httpcode: 404 ❌
url: /api/company/{id} | returns: nothing    | returns: nothing

### Disconnect movie:
Request:                                | Response - ok:      | Response - not found: | 
--------------------------------------- | ------------------- | --------------------- |
method: DELETE                          | httpcode: 200 ✔️    | httpcode: 404 ❌
url: /api/company/{id}/movies/{movieId} | returns: nothing    | returns: errors

--------------------------------------------

## Crew member
### Create crew member:
- expects: ```{ characterName: string, role: int, movieId: int, personId: int }```
- httpcode: 200 ✔️
- method: POST
- url: /api/crewmember
- returns: crew member


### Get crew members:
- httpcode: 200 ✔️
- method: GET
- url: /api/crewmember
- returns: list of crew members

### Get crew member:
- httpcode: 200 ✔️
- method: GET
- url: /api/crewmember/{id}
- returns: crew member

### Update crew member:
- expects: ```{ id: int, characterName: string, role: int, movieId: int, personId: int }```
- httpcode: 200 ✔️
- method: PUT
- url: /api/crewmember/{id}
- returns: crew member

### Delete crew member:
- httpcode: 200 ✔️
- method: DELETE
- url: /api/crewmember/{id}
- returns: nothing


--------------------------------------------

## Genre
### Create genre:
- expects: ```{ name: string }```
- httpcode: 200 ✔️
- method: POST
- url: /api/genre
- returns: genre

### Get genres:
- httpcode: 200 ✔️
- method: GET
- url: /api/genre
- returns: list of genres

### Get genre:
- httpcode: 200 ✔️
- method: GET
- url: /api/genre/{id}
- returns: genre

### Update genre:
- expects: ```{ id: int, name: string }```
- httpcode: 200 ✔️
- method: PUT
- url: /api/genre/{id}
- returns: genre

### Delete genre:
- httpcode: 200 ✔️
- method: DELETE
- url: /api/genre/{id}
- returns: nothing

--------------------------------------------

## Language
### Create language:
- expects: ```{ name: string }```
- httpcode: 200 ✔️
- method: POST
- url: /api/language
- returns: language

### Get languages:
- httpcode: 200 ✔️
- method: GET
- url: /api/language
- returns: list of languages

### Get language:
- httpcode: 200 ✔️
- method: GET
- url: /api/language/{id}
- returns: language

### Update language:
- expects: ```{ id: int, name: string }```
- httpcode: 200 ✔️
- method: PUT
- url: /api/language/{id}
- returns: language

### Delete language:
- httpcode: 200 ✔️
- method: DELETE
- url: /api/language/{id}
- returns: nothing

--------------------------------------------

## Movie
### Create movie:
- expects: ```{ description: string, languageId: int, length: int, name: string, releaseDate: date }```
- httpcode: 200 ✔️
- method: POST
- url: /api/movie
- returns: movie

### Connect genre:
- expects: ```{ genreId: int }```
- httpcode: 200 ✔️
- method: POST
- url: /api/movie/{id}/genres
- returns: movie

### Get movies:
- httpcode: 200 ✔️
- method: GET
- url: /api/movie
- returns: list of movies

### Get movie:
- httpcode: 200 ✔️
- method: GET
- url: /api/movie/{id}
- returns: movie

### Update movie:
- expects: ```{ id: int, description: string, languageId: int, length: int, name: string, releaseDate: date }```
- httpcode: 200 ✔️
- method: PUT
- url: /api/movie/{id}
- returns: movie

### Delete movie:
- httpcode: 200 ✔️
- method: DELETE
- url: /api/movie/{id}
- returns: nothing

### Disconnect genre:
- httpcode: 200 ✔️
- method: DELETE
- url: /api/movie/{id}/genres{genreId}
- returns: nothing

--------------------------------------------

## Person
### Create person:
- expects: ```{ birthDate: date, birthPlace: string, description: string, firstName: string, lastName: string }```
- httpcode: 200 ✔️
- method: POST
- url: /api/person
- returns: person

### Get persons:
- httpcode: 200 ✔️
- method: GET
- url: /api/person
- returns: list of persons

### Get person:
- httpcode: 200 ✔️
- method: GET
- url: /api/person/{id}
- returns: person

### Update person:
- expects: ```{ id: int, birthDate: date, birthPlace: string, description: string, firstName: string, lastName: string }```
- httpcode: 200 ✔️
- method: PUT
- url: /api/person/{id}
- returns: person

### Delete person:
- httpcode: 200 ✔️
- method: DELETE
- url: /api/person/{id}
- returns: nothing
