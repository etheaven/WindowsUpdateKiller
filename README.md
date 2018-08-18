# WindowsUpdateKiller
A service to kill windows update anytime it starts

As there was no way to really disable windows update from making itself back Manual from Disabled status, I had to make this service because there was no such solution available  
I dislike Microsoft from doing this and will do anything to prevent Windows Update happenning on my machine again

# Running the service
All you need to do is compile the source and then launch cmd as Administrator then type `WinUpdateKiller.exe -install`

# Uninstalling the service
To uninstall the service you only need to type `WinUpdateKiller.exe -uninstall` into the cmd ran as Administrator
