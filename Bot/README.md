# so-un-bot-for-real

The code behind so-un-bot

Nothing is documnted so far.
I will try to write a documentation on my free time.
Have fun!

## How to run

#### Prepare the environment
```
$ dotnet restore
```

#### Option 1: using the .NET runtime (and SDK) in your host
```
$ dotnet run
```
*This will compile the project in debug mode*

#### Option 2: in Docker
```
$ dotnet publish -c Release
$ docker build -t sounbot .
$ docker run -d --name='sounbot' -v '/path/to/datadir':'/App/ACL':'rw' 'sobot:latest'
```

Remember to change `/path/to/datadir` with your own data directory!

## How to debug

Just open the project in **Visual Studio** or **VSCode** with the **.NET Extension Pack** extension.

You will need the .NET 6.0 SDK: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
