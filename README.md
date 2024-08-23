Application startup in Visual Studio 2022
  * Application starts in console
  * waits for input to send message, eg Node mymessage

Architecture 
  * Application initialization and startup – starts the program, configure DI services, and initialize the application
  * Central Hub – initializes nodes and coordinates between nodes
  * Dependent Nodes – generates events and messages for communication with central hub and nodes
  * Storage – In memory storage of communication messages and events. Can be replaced with other types if necessary 

Program Classes and Interfaces 
  * Interfaces 
    * ICentralHub
    * IDependentHub
    * IStorageProvider 
  * Classes
    * CentralHub : ICentralHub
    * DependentNode : IDenpendentNode
    * InMemoryStorage : IStorageProvider
    * Message
    * Event 

Program/Application 
  * Program
  * Startup
  * Application 

 
