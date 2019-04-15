# DotNetCoreMvcWebApi-Example
There is a brief example of MVC and WebApi on .Net Core

Here, there are some design decision for the project and their reasons.

Project is developed and tested on windows 10 64 bit computer by using .net core 2.0 and Visual Studio 2017. 
SQL Server is used as relational database. Database scripts are accessable from 'DB Scripts' folder. 

● Which patterns did you use and why did you choose those patterns? 

  In the project, if the searched movie data is not found in the database, by helping an rest api source, 
  movie info are obtained and stored to project's database. Because of changeability of api source, it is 
  necessary to separate the data(api) layer from the logic layer, so, strategy, factory and singleton 
  patterns are used to realize this purpose.
  
  All different api sources are managed by an interface class IMovieRepo. It has a getMovie method which 
  takes movie's title as parameter and returns MovieClass object.(Strategy Pattern)
  
  MovieRepoFactory class is the factory class which generate the api source wanted to be used to get 
  searched movie info and it provides a separation between data layer and logic layer. 
  
  In addition to that, to prevent factory class from creation of multiple instance, MovieRepoFactory is 
  developed as singleton.
  
● Which DI tools did you use and why did you choose that tool?

  Periodical tasks are handled by MovieDataRefreshService class and it has to be one instance. So, singleton
  dependency service is used.
