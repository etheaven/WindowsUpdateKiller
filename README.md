# WindowsUpdateKiller
A service to kill windows update anytime it starts

As there was no way to really disable windows update from making itself back Manual from Disabled status, I had to make this service because there was no such solution available  
I dislike Microsoft from doing this and will do anything to prevent Windows Update happenning on my machine again

# What this does? / How it works?
This service repeatedly tries to first `Stop` the windows update then make it `"Disabled"` in `services.msc` so it will never realy bother you. That's all it does:)

# Running the service
All you need to do is compile the source and then launch cmd as Administrator then type `WinUpdateKiller.exe -install` and start the service in `services.msc`  
The next time you start your computer the service is running and doing it's job:)

# Uninstalling the service
To uninstall the service you only need to type `WinUpdateKiller.exe -uninstall` into the cmd ran as Administrator
