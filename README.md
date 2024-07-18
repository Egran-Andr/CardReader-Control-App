# Checkpoint WPF app with usage of RFID cards
This is an WPF Application that i tried using to read and write on RFID and NFC cards.

## RFID-Library
The main idea of this project is creation of  easy-to-use RFID connection library. You can easely use it by importing NFCReader.cs and Card.cs files.
The library was tested on USB ARC122U Rfid Reader,but theoreticly will work with other models.

### Connection

```csharp
//Initializing
NFCReader NFC = new NFCReader();

//Connecting
NFC.Connect(); // public bool Connect()

//Disconnecting
NFC.Disconnect(); // public void Disconnect()
```
```csharp
//Inserted Event 
NFC.CardInserted += new NFCReader.CardEventHandler(...Some function);

//Ejected Event
NFC.CardEjected += new NFCReader.CardEventHandler(... Some function);

//Enabling Event Watching
NFC.Watch(); //public void Watch()
```

### Read, Write Authorize

```csharp
//Authorizing
NFC.AuthBlock("2"); // private bool AuthBlock(String block)

//Reading
NFC.ReadBlock("2"); //public byte[] ReadBlock(String Block)

//Writing   
NFC.WriteBlock("Some string data that will not exceed block limit", "2"); //public bool WriteBlock(String Text, String Block)
```
### ReaderList, CardUID

```csharp
//Card UID
NFC.GetCardUID();

//Available Readers connected to device 
NFC.GetReadersList(); //public List<string> GetReadersList()
```

## Example Inserted and Ejected Event Usage
```csharp
public void Card_Inserted()
{
  try
  {
    if (NFC.Connect())
    {
        //Do stuff like NFC.GetCardUID(); ...
    }
    else
    {
        //Give error message about connection...
    }
  }
}
```

```csharp
public void Card_Ejected()
{
   //Do stuff...
   NFC.Disconnect();
}
```

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
First,this project wont work without Web API.  Base code to Api(python FastApi)  is located in project : https://github.com/Egran-Andr/Python-WebApiForRFID
To change adresss oe port connection you must change file ApiHelper.cs.


###Equipment
Project was tested and working with ACR 122u reader but it should work with any reader with USB port conection.

###Installing
A step by step series of examples that tell you how to get a development env running

git clone https://github.com/Egran-Andr/CardReder-Control-App cd 

## Built With

* [HttpClient]([https://bottlepy.org/](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler?view=net-7.0)https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler?view=net-7.0) - HttpClient Docs
* [Material design for WPF](http://materialdesigninxaml.net/) - XAML Material Docs
